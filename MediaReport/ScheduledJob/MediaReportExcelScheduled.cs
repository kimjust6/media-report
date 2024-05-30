using System.Text;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using ClosedXML.Excel;
using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;


namespace Alloy.MediaReport.ScheduledJob;

[ScheduledPlugIn(GUID = "fba2911b-9c97-46d5-8232-d3f9a43c03ed",
    DisplayName = "Media Report Excel Generator",
    Description = "Creates the excel version of the media report for download",
    Restartable = true,
    DefaultEnabled = true,
    IntervalLength = 1,
    IntervalType = ScheduledIntervalType.Days)]
[ServiceConfiguration(IncludeServiceAccessor = false)]
public class MediaReportExcelScheduled : ScheduledJobBase
{
    private bool _isStopped = false;
    private readonly IMediaReportDdsRepository _mediaReportDdsRepository;
    private readonly IContentLoader _contentLoader;


    public MediaReportExcelScheduled(
        IMediaReportDdsRepository mediaReportDdsRepository,
        IContentLoader contentLoader)
    {
        _mediaReportDdsRepository = mediaReportDdsRepository;
        _contentLoader = contentLoader;
        IsStoppable = true;
    }

    public override string Execute()
    {
        //get baseurl of site
        var currentSite = SiteDefinition.Current;
        if (currentSite == null || ContentReference.IsNullOrEmpty(currentSite.StartPage))
        {
            currentSite = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>().List().FirstOrDefault();
        }

        if (currentSite == null)
        {
            throw new Exception("No sites defined");
        }

        var baseUrl = currentSite.SiteUrl.ToString();
        if (baseUrl.EndsWith('/'))
        {
            baseUrl = baseUrl.TrimEnd('/');
        }

        var countProcessedItems = 0;
        _isStopped = false;
        //open a new workbook
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Report");

            // get the values from dds
            var allDdsItems = _mediaReportDdsRepository.ListAll().ToList();
            foreach (var mediaReportDdsItem in allDdsItems)
            {
                if (_isStopped)
                {
                    return "The job was stopped";
                }

                // convert to excel
                if (_contentLoader.TryGet<IContent>(mediaReportDdsItem.ContentLink, out _))
                {
                    ++countProcessedItems;
                    // start from Col A
                    var col = 'A';
                    foreach (var propInfo in mediaReportDdsItem.GetType().GetProperties())
                    {
                        if (countProcessedItems == 1)
                        {
                            worksheet.Cell(col.ToString() + countProcessedItems).Value = propInfo.Name;
                        }
                        else if (propInfo.Name == "References")
                        {
                            var myReferences = propInfo.GetValue(mediaReportDdsItem, null) + "";
                            var split = myReferences.Split(',');
                            StringBuilder sb = new();

                            foreach (var val in split)
                            {
                                sb.Append(baseUrl + "/EPiServer/CMS/#context=epi.cms.contentdata:///");
                                sb.Append(val);
                                sb.Append(',');
                            }
                            // trim last comma
                            if (sb.Length > 0)
                            {
                                --(sb.Length);
                            }

                            worksheet.Cell(col.ToString() + countProcessedItems).Value = sb.ToString();
                        }
                        else
                        {
                            worksheet.Cell(col.ToString() + countProcessedItems).Value =
                                (propInfo.GetValue(mediaReportDdsItem, null) ?? "null").ToString();
                        }

                        col = (char)(col + 1);
                    }
                }
                else
                {
                    _mediaReportDdsRepository.Delete(mediaReportDdsItem.ContentLink);
                }

                // save to file
                try
                {
                    workbook.SaveAs("./wwwroot/MediaReport.xlsx");
                }
                catch (Exception e)
                {
                    Stop();
                    throw new Exception("Failed to open MediaReport.xlsx: " + e.Message);
                }

                // upload file to bucket storage
            }
        }

        return $"Job completed ({countProcessedItems} media content processed)";
    }

    public override void Stop()
    {
        base.Stop();
        _isStopped = true;
    }
}