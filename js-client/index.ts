/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface CreateTaskEntryRequest {
  title: string;
  /** @format date-time */
  startTime: string;
  /** @format date-time */
  endTime: string;
  /** @format int64 */
  projectId: number;
  taskId: string;
  description: string;
}

export interface CreateTaskEntryResponse {
  /** @format int64 */
  newTaskEntryId: number;
}

export interface CreateUnwellEntryRequest {
  /** @format date-time */
  startTime: string;
  /** @format date-time */
  endTime: string;
}

export interface CreateUnwellResponse {
  /** @format int64 */
  newUnwellEntryId: number;
}

export interface EmployeeDto {
  /** @format int64 */
  id: number;
  fullName: string;
}

export interface EmployeeTrackedTaskHourDto {
  /** @format int64 */
  employeeId: number;
  /** @format double */
  trackedHours: number;
}

export type EntryType = number;

export interface GetAllEmployeesResponse {
  employees: EmployeeDto[];
}

export interface GetAllProjectsResponse {
  projects: ProjectDto[];
}

export interface GetEmployeesTrackedTaskHoursResponse {
  employeesTrackedTaskHours: EmployeeTrackedTaskHourDto[];
}

export interface GetEntriesByPeriodResponse {
  taskEntries: TaskEntryDto[];
  unwellEntries: UnwellEntryDto[];
}

export interface GetPersonalReportResponse {
  trackedEntries: TrackedEntryDto[];
  /** @format double */
  taskHours: number;
  /** @format double */
  unwellHours: number;
}

export interface ProjectDto {
  /** @format int64 */
  id: number;
  name: string;
}

export type ProjectDto2 = {
  /** @format int64 */
  id: number;
  name: string;
} | null;

export interface ProjectsResponse {
  projects: ProjectDto[];
}

export interface SoftDeleteEntryRequest {
  /** @format int64 */
  id?: number;
  deletionReason: string;
}

export type TaskDto = {
  /** @format int64 */
  id: number;
  title: string;
} | null;

export interface TaskEntryDto {
  /** @format int64 */
  id: number;
  title: string;
  /** @format date-time */
  startTime: string;
  /** @format date-time */
  endTime: string;
  type: EntryType;
  /** @format int64 */
  projectId: number;
  taskId: string;
  description: string;
}

export interface TrackedEntryDto {
  /** @format int64 */
  id: number;
  /** @format double */
  trackedHoursPerDay: number;
  /** @format date-time */
  startTime: string;
  /** @format date-time */
  endTime: string;
  entryType: EntryType;
  project: ProjectDto2;
  task: TaskDto;
  description?: string | null;
}

export interface UnwellEntryDto {
  /** @format int64 */
  id: number;
  /** @format date-time */
  startTime: string;
  /** @format date-time */
  endTime: string;
  type: EntryType;
}

export interface UpdateTaskEntryRequest {
  /** @format int64 */
  id?: number;
  title: string;
  /** @format date-time */
  startTime: string;
  /** @format date-time */
  endTime: string;
  /** @format int64 */
  projectId: number;
  taskId: string;
  description: string;
}

export interface UpdateUnwellEntryRequest {
  /** @format int64 */
  id?: number;
  /** @format date-time */
  startTime: string;
  /** @format date-time */
  endTime: string;
}

import type {
  AxiosInstance,
  AxiosRequestConfig,
  AxiosResponse,
  HeadersDefaults,
  ResponseType,
} from "axios";
import axios from "axios";

export type QueryParamsType = Record<string | number, any>;

export interface FullRequestParams
  extends Omit<AxiosRequestConfig, "data" | "params" | "url" | "responseType"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseType;
  /** request body */
  body?: unknown;
}

export type RequestParams = Omit<
  FullRequestParams,
  "body" | "method" | "query" | "path"
>;

export interface ApiConfig<SecurityDataType = unknown>
  extends Omit<AxiosRequestConfig, "data" | "cancelToken"> {
  securityWorker?: (
    securityData: SecurityDataType | null,
  ) => Promise<AxiosRequestConfig | void> | AxiosRequestConfig | void;
  secure?: boolean;
  format?: ResponseType;
}

export enum ContentType {
  Json = "application/json",
  JsonApi = "application/vnd.api+json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public instance: AxiosInstance;
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private secure?: boolean;
  private format?: ResponseType;

  constructor({
    securityWorker,
    secure,
    format,
    ...axiosConfig
  }: ApiConfig<SecurityDataType> = {}) {
    this.instance = axios.create({
      ...axiosConfig,
      baseURL: axiosConfig.baseURL || "http://localhost:6507/",
    });
    this.secure = secure;
    this.format = format;
    this.securityWorker = securityWorker;
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected mergeRequestParams(
    params1: AxiosRequestConfig,
    params2?: AxiosRequestConfig,
  ): AxiosRequestConfig {
    const method = params1.method || (params2 && params2.method);

    return {
      ...this.instance.defaults,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...((method &&
          this.instance.defaults.headers[
            method.toLowerCase() as keyof HeadersDefaults
          ]) ||
          {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected stringifyFormItem(formItem: unknown) {
    if (typeof formItem === "object" && formItem !== null) {
      return JSON.stringify(formItem);
    } else {
      return `${formItem}`;
    }
  }

  protected createFormData(input: Record<string, unknown>): FormData {
    if (input instanceof FormData) {
      return input;
    }
    return Object.keys(input || {}).reduce((formData, key) => {
      const property = input[key];
      const propertyContent: any[] =
        property instanceof Array ? property : [property];

      for (const formItem of propertyContent) {
        const isFileType = formItem instanceof Blob || formItem instanceof File;
        formData.append(
          key,
          isFileType ? formItem : this.stringifyFormItem(formItem),
        );
      }

      return formData;
    }, new FormData());
  }

  public request = async <T = any, _E = any>({
    secure,
    path,
    type,
    query,
    format,
    body,
    ...params
  }: FullRequestParams): Promise<AxiosResponse<T>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const responseFormat = format || this.format || undefined;

    if (
      type === ContentType.FormData &&
      body &&
      body !== null &&
      typeof body === "object"
    ) {
      body = this.createFormData(body as Record<string, unknown>);
    }

    if (
      type === ContentType.Text &&
      body &&
      body !== null &&
      typeof body !== "string"
    ) {
      body = JSON.stringify(body);
    }

    return this.instance.request({
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type ? { "Content-Type": type } : {}),
      },
      params: query,
      responseType: responseFormat,
      data: body,
      url: path,
    });
  };
}

/**
 * @title inner-circle-time-api
 * @version 1.3.0
 * @baseUrl http://localhost:6507/
 */
export class Api<
  SecurityDataType extends unknown,
> extends HttpClient<SecurityDataType> {
  api = {
    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingGetEntriesByPeriod
     * @summary Get entries by period
     * @request GET:/api/tracking/entries
     */
    trackingGetEntriesByPeriod: (
      query: {
        /** @format date */
        startDate: string;
        /** @format date */
        endDate: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<GetEntriesByPeriodResponse, any>({
        path: `/api/tracking/entries`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingCreateTaskEntry
     * @summary Create a task entry
     * @request POST:/api/tracking/task-entries
     */
    trackingCreateTaskEntry: (
      data: CreateTaskEntryRequest,
      params: RequestParams = {},
    ) =>
      this.request<CreateTaskEntryResponse, any>({
        path: `/api/tracking/task-entries`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingCreateUnwellEntry
     * @summary Create an unwell entry
     * @request POST:/api/tracking/unwell-entries
     */
    trackingCreateUnwellEntry: (
      data: CreateUnwellEntryRequest,
      params: RequestParams = {},
    ) =>
      this.request<CreateUnwellResponse, any>({
        path: `/api/tracking/unwell-entries`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingUpdateTaskEntry
     * @summary Update a task entry
     * @request POST:/api/tracking/task-entries/{taskEntryId}
     */
    trackingUpdateTaskEntry: (
      taskEntryId: number,
      data: UpdateTaskEntryRequest,
      params: RequestParams = {},
    ) =>
      this.request<void, any>({
        path: `/api/tracking/task-entries/${taskEntryId}`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingUpdateUnwellEntry
     * @summary Update an unwell entry
     * @request POST:/api/tracking/unwell-entries/{unwellEntryId}
     */
    trackingUpdateUnwellEntry: (
      unwellEntryId: number,
      data: UpdateUnwellEntryRequest,
      params: RequestParams = {},
    ) =>
      this.request<void, any>({
        path: `/api/tracking/unwell-entries/${unwellEntryId}`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingGetEmployeeProjectsByPeriod
     * @summary Get employee projects by period
     * @request GET:/api/tracking/task-entries/projects
     */
    trackingGetEmployeeProjectsByPeriod: (
      query: {
        /** @format date */
        startDate: string;
        /** @format date */
        endDate: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<ProjectsResponse, any>({
        path: `/api/tracking/task-entries/projects`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingHardDeleteEntry
     * @summary Deletes specific entry
     * @request DELETE:/api/tracking/entries/{entryId}/hard-delete
     */
    trackingHardDeleteEntry: (entryId: number, params: RequestParams = {}) =>
      this.request<void, any>({
        path: `/api/tracking/entries/${entryId}/hard-delete`,
        method: "DELETE",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Tracking
     * @name TrackingSoftDeleteEntry
     * @summary Soft deletes specific entry
     * @request DELETE:/api/tracking/entries/{entryId}/soft-delete
     */
    trackingSoftDeleteEntry: (
      entryId: number,
      data: SoftDeleteEntryRequest,
      params: RequestParams = {},
    ) =>
      this.request<void, any>({
        path: `/api/tracking/entries/${entryId}/soft-delete`,
        method: "DELETE",
        body: data,
        type: ContentType.Json,
        ...params,
      }),

    /**
     * No description
     *
     * @tags Reporting
     * @name ReportingGetAllEmployees
     * @summary Get all employees
     * @request GET:/api/reporting/employees
     */
    reportingGetAllEmployees: (params: RequestParams = {}) =>
      this.request<GetAllEmployeesResponse, any>({
        path: `/api/reporting/employees`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Reporting
     * @name ReportingGetPersonalReport
     * @summary Get personal report
     * @request GET:/api/reporting/personal-report
     */
    reportingGetPersonalReport: (
      query: {
        /** @format int64 */
        employeeId: number;
        /** @format int32 */
        year: number;
        /** @format int32 */
        month: number;
      },
      params: RequestParams = {},
    ) =>
      this.request<GetPersonalReportResponse, any>({
        path: `/api/reporting/personal-report`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Internal
     * @name InternalGetEmployeesTrackedTaskHours
     * @summary Get employees tracked task hours
     * @request GET:/api/internal/projects/tracked-task-hours
     */
    internalGetEmployeesTrackedTaskHours: (
      query: {
        /** @format int64 */
        projectId: number;
        /** @format date */
        startDate: string;
        /** @format date */
        endDate: string;
      },
      params: RequestParams = {},
    ) =>
      this.request<GetEmployeesTrackedTaskHoursResponse, any>({
        path: `/api/internal/projects/tracked-task-hours`,
        method: "GET",
        query: query,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Internal
     * @name InternalGetAllProjects
     * @summary Get all projects
     * @request GET:/api/internal/projects
     */
    internalGetAllProjects: (params: RequestParams = {}) =>
      this.request<GetAllProjectsResponse, any>({
        path: `/api/internal/projects`,
        method: "GET",
        format: "json",
        ...params,
      }),
  };
}
