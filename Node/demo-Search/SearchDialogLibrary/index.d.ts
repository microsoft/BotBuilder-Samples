import { Library } from "botbuilder";

// Exported functions
export function create(libraryId: string, settings: SearchDialogSettings): Library;
export function defaultResultsMapper(itemMap: ItemMapper): SearchResultsMapper;

declare type ItemMapper = (input: any) => SearchHit;
declare type SearchResultsMapper = (providerResults: SearchResults) => SearchResults;
declare type RefineFormatter = (refiners: Array<string>) => any;

// Entities
export interface SearchDialogSettings {
  search: (query: Query) => PromiseLike<SearchResults>;
  pageSize?: number;
  multipleSelection?: boolean;
  refiners?: Array<string>;
  refineFormatter?: RefineFormatter;
}

export interface Query {
  searchText: string;
  pageSize: number;
  pageNumber: number;
  filters: Array<FacetFilter>;
  facets: Array<string>;
}

export interface SearchResults {
  facets: Array<Facet>;
  results: Array<SearchHit>
}

export interface SearchHit {
  key: string;
  title: string;
  description: string;
  imageUrl?: string
}

export interface FacetFilter {
  key: string;
  value: any;
}

export interface Facet {
  key: string;
  options: Array<FacetValue>;
}

export interface FacetValue {
  value: string;
  count: number;
}