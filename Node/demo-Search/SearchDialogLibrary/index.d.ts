import { Library, Session } from "botbuilder";

// exported functions
export function create(settings: ISearchDialogSettings): Library;
export function begin(session: Session, args?: any): void;
export function refine(session: Session, args?: any): void;
export function defaultResultsMapper(itemMap: ItemMapper): SearchResultsMapper;

declare type ItemMapper = (input: any) => ISearchHit;
declare type SearchResultsMapper = (providerResults: ISearchResults) => ISearchResults;
declare type RefineFormatter = (refiners: Array<string>) => any;

// entities
export interface ISearchDialogSettings {
  search: (query: IQuery) => PromiseLike<ISearchResults>;
  pageSize?: number;
  multipleSelection?: boolean;
  refiners?: Array<string>;
  refineFormatter?: RefineFormatter;
}

export interface IQuery {
  searchText: string;
  pageSize: number;
  pageNumber: number;
  filters: Array<IFacetFilter>;
  facets: Array<string>;
}

export interface ISearchResults {
  facets: Array<IFacet>;
  results: Array<ISearchHit>;
}

export interface ISearchHit {
  key: string;
  title: string;
  description: string;
  imageUrl?: string;
}

export interface IFacetFilter {
  key: string;
  value: any;
}

export interface IFacet {
  key: string;
  options: Array<IFacetValue>;
}

export interface IFacetValue {
  value: string;
  count: number;
}