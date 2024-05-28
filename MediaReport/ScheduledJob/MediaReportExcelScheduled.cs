using EPiServer;
using EPiServer.Cms.Shell.UI.Rest.Capabilities;
using EPiServer.Cms.Shell.UI.Rest.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Web.Mvc;


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
    private readonly MediaDtoConverter _mediaDtoConverter;
    private readonly IMediaReportDdsRepository _mediaReportDdsRepository;
    private readonly IMediaLoader _mediaLoader;
    private readonly IMediaReportItemsSumDdsRepository _mediaReportItemsSumDdsRepository;
    private readonly IMediaSizeResolver _mediaSizeResolver;
    private readonly IContentCapability _isLocalContent;
    private readonly ReferencedContentResolver _referencedContentResolver;
    private readonly IContentLoader _contentLoader;
    private readonly IMediaHierarchyRootResolver _mediaHierarchyRootResolver;

    public MediaReportExcelScheduled(IMediaReportDdsRepository mediaReportDdsRepository, IMediaLoader mediaLoader,
        IMediaSizeResolver mediaSizeResolver, IEnumerable<IContentCapability> capabilities,
        IContentLoader contentLoader, ReferencedContentResolver referencedContentResolver,
        IMediaReportItemsSumDdsRepository mediaReportItemsSumDdsRepository,
        IMediaHierarchyRootResolver mediaHierarchyRootResolver,
        MediaDtoConverter mediaDtoConverter)
    {
        _mediaReportDdsRepository = mediaReportDdsRepository;
        _mediaLoader = mediaLoader;
        _mediaSizeResolver = mediaSizeResolver;
        _isLocalContent = capabilities.Single(x => x.Key == "isLocalContent");
        _contentLoader = contentLoader;
        _referencedContentResolver = referencedContentResolver;
        _mediaReportItemsSumDdsRepository = mediaReportItemsSumDdsRepository;
        _mediaHierarchyRootResolver = mediaHierarchyRootResolver;
        IsStoppable = true;
        _mediaDtoConverter = mediaDtoConverter;
    }

    public override string Execute()
    {
        var countProcessedItems = 0;
        _isStopped = false;

        var updatedList = new List<ContentReference>();
        var itemsSum = MediaReportItemsSum.Empty();

        // get the values
        var allDdsItems = _mediaReportDdsRepository.ListAll().ToList();
        foreach (var mediaReportDdsItem in allDdsItems)
        {
            if (_isStopped)
            {
                return "The job was stopped";
            }


            if (_contentLoader.TryGet<IContent>(mediaReportDdsItem.ContentLink, out _))
            {
                ++countProcessedItems;
                // convert to excel
                
                // save to file

                // upload file to dds
            }
            // else
            // {
            //     _mediaReportDdsRepository.Delete(mediaReportDdsItem.ContentLink);
            // }
        }

        // var items = _mediaReportDdsRepository.Search(null, null, null, null,
        //     1, 1000, null, null, null, null, out var totalCount).ToList();
        // var result = items.Select(_mediaDtoConverter.Convert).ToList();
        // var nice = new JsonDataResult(new { items = result, filterRange = mediaReportItemsSum, totalCount });
        return $"Job completed ({countProcessedItems} media content processed)";
    }

    public override void Stop()
    {
        base.Stop();
        _isStopped = true;
    }
}