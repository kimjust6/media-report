import React, { useEffect, useState } from "react";
import { Button, Grid, MenuItem, Select, Slider, Typography } from "@mui/material";
import { FilterRange } from "./models";
import { formatBytes } from "./format-bytes";

const IS_LOCAL_ALL = "All";
const IS_LOCAL_ONLY_LOCAL = "Only local";
const IS_LOCAL_ONLY_SHARED = "Only shared";
const localFilterOptions: string[] = [IS_LOCAL_ALL, IS_LOCAL_ONLY_LOCAL, IS_LOCAL_ONLY_SHARED];

export type OnFilterChangeHandler = (
    minSize: number,
    maxSize: number,
    minReferences: number,
    maxReferences: number,
    isLocal?: boolean
) => void;

interface ListFilter {
    filterRange: FilterRange;
    onFilterChange: OnFilterChangeHandler;
}

interface FilterControl {
    label: string;
    minValue?: string;
    maxValue?: string;
    children: any;
}

function FilterControl({ label, minValue, maxValue, children }: FilterControl) {
    return (
        <label>
            <Grid container gap={1} alignItems="center">
                <Grid item>
                    <Typography fontWeight="bold">{label}:</Typography>
                </Grid>
                {minValue && <Grid item>{minValue}</Grid>}
                <Grid item>{children}</Grid>
                {maxValue && <Grid item>{maxValue}</Grid>}
            </Grid>
        </label>
    );
}

export default function ListFilter({ filterRange, onFilterChange }: ListFilter) {
    const [isLocal, setIsLocal] = useState<string>(IS_LOCAL_ALL);
    const [size, setSize] = useState<number[]>([0, 10000]);
    const [references, setReferences] = useState<number[]>([20, 37]);

    useEffect(() => {
        if (!filterRange) {
            return;
        }

        if (filterRange.maxSize > 0) {
            setSize([filterRange.minSize, filterRange.maxSize]);
        }

        if (filterRange.maxReferences > 0) {
            setReferences([filterRange.minReferences, filterRange.maxReferences]);
        }
    }, [filterRange]);

    const handleSize = (event: Event, newValue: number | number[]) => {
        setSize(newValue as number[]);
    };

    const handleReferences = (event: Event, newValue: number | number[]) => {
        setReferences(newValue as number[]);
    };

    const handleChangeIsLocal = (localContent: string) => {
        setIsLocal(localContent);
    };

    const getIsLocalFilterValue = (): boolean | undefined => {
        switch (isLocal) {
            case IS_LOCAL_ALL:
                return undefined;
            case IS_LOCAL_ONLY_SHARED:
                return false;
            case IS_LOCAL_ONLY_LOCAL:
                return true;
        }
        return undefined;
    };

    const onSearch = () => {
        onFilterChange(size[0], size[1], references[0], references[1], getIsLocalFilterValue());
    };

    return (
        <Grid container alignItems="baseline" gap={3}>
            {filterRange?.maxSize > 0 && (
                <Grid item>
                    <FilterControl
                        label="Size"
                        minValue={formatBytes(filterRange.minSize)}
                        maxValue={formatBytes(filterRange.maxSize)}
                    >
                        <Slider
                            aria-label="Size"
                            value={size}
                            onChange={handleSize}
                            valueLabelDisplay="auto"
                            valueLabelFormat={(value: number) => formatBytes(value)}
                            sx={{
                                width: 250,
                                marginX: 2
                            }}
                            min={filterRange.minSize}
                            max={filterRange.maxSize}
                        />
                    </FilterControl>
                </Grid>
            )}

            {filterRange?.maxReferences > 0 && (
                <Grid item>
                    <FilterControl
                        label="References"
                        minValue={filterRange.minReferences.toString()}
                        maxValue={filterRange.maxReferences.toString()}
                    >
                        <Slider
                            aria-label="References"
                            value={references}
                            valueLabelDisplay="auto"
                            onChange={handleReferences}
                            min={filterRange.minReferences}
                            max={filterRange.maxReferences}
                            sx={{
                                width: 250,
                                marginX: 2
                            }}
                        />
                    </FilterControl>
                </Grid>
            )}

            <Grid item>
                <FilterControl label="Local content">
                    <Select
                        labelId="media-report-filter-is-local"
                        id="demo-simple-select"
                        value={isLocal}
                        label="Age"
                        autoWidth={false}
                        sx={{
                            width: 150,
                        }}
                        onChange={(event) => handleChangeIsLocal(event.target.value)}
                    >
                        {localFilterOptions.map((x) => (
                            <MenuItem key={x} value={x}>
                                {x}
                            </MenuItem>
                        ))}
                    </Select>
                </FilterControl>
            </Grid>
            <Grid item>
                <Button variant="contained" onClick={onSearch}>
                    Search
                </Button>
            </Grid>
        </Grid>
    );
}
