// types/import.ts
export type ImportStatus = 'Pending' | 'Processing' | 'Completed' | 'Failed';

export interface ImportJob {
  importJobId: string;
  fileName: string;
  totalRows: number;
  successCount: number;
  failureCount: number;
  status: ImportStatus;
  startedAt: string;
  completedAt?: string;
  createdBy?: string;
  errors?: ImportError[];
}

export interface ImportError {
  row: number;
  field?: string;
  rawValue?: string;
  message: string;
}

export interface PagedResult<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
