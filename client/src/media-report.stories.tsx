import React from "react";
import { ComponentStory, ComponentMeta } from "@storybook/react";
import { MediaReportComponent } from "./media-report";
import data from "./__tests__/media-report-data.json";
import { FilterRange } from "./models";

export default {
    title: "Media report",
    component: MediaReportComponent,
    argTypes: {
        items: [],
    },
} as ComponentMeta<typeof MediaReportComponent>;

const getDefaultFilter = (): FilterRange => ({
    minSize: 250,
    maxSize: 2133232,

    minModifiedDate: new Date(),
    maxModifiedDate: new Date(),

    minReferences: 0,
    maxReferences: 11,
});

const Template: ComponentStory<typeof MediaReportComponent> = (args) => <MediaReportComponent {...args} />;

export const Default = Template.bind({});
Default.args = {
    items: data,
    filterRange: getDefaultFilter(),
    totalCount: 10,
    onFilterChange: (minSize) => alert(minSize),
    onPageChange: (pageIndex) => alert(pageIndex)
};

export const LongList = Template.bind({});
LongList.args = {
    items: [...Array(300).keys()].map((index) => ({
        name: "Media " + (index + 1),
        contentLink: (index + 1).toString(),
        editUrl: "https://www.google.com",
        publicUrl: "",
        contentTypeName: "test",
        hierarchy: [
            {
                key: "123",
                value: "Test",
            },
            {
                key: "1234",
                value: "Test2",
            },
            {
                key: "12345",
                value: "Test3",
            },
        ],
        thumbnailUrl: "https://loremflickr.com/320/240?random=" + index,
        lastModified: "2023-01-01 20:22",
        size: (index + 1) * 1000,
        isLocalContent: index % 5 === 1,
        references: [],
        errorText: ""
    })),
    filterRange: getDefaultFilter(),
    totalCount: 500,
    onFilterChange: (minSize) => alert(minSize),
    onPageChange: (pageIndex: number) => alert(pageIndex)
};

export const Empty = Template.bind({});
Empty.args = {
    items: [],
    filterRange: getDefaultFilter(),
    totalCount: 0,
    onFilterChange: (minSize) => alert(minSize),
    onPageChange: (pageIndex: number) => alert(pageIndex)
};
