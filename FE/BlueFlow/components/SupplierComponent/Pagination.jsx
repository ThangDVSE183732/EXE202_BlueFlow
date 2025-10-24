import React, { useMemo } from 'react';
import ReactPaginate from 'react-paginate';

/**
 * ReusablePagination
 * Props:
 *  total:        tổng số items
 *  page:         trang hiện tại (1-based)
 *  pageSize:     số item mỗi trang
 *  onPageChange: (nextPageNumber) => void  (1-based)
 *  pageSizeOptions?: number[]  (ví dụ [6,9,12])
 *  onPageSizeChange?: (newSize) => void
 *  showSummary?: boolean
 *  showFirstLast?: boolean
 *  siblingCount?: số trang lân cận (map sang pageRangeDisplayed)
 *  boundaryCount?: số trang ở đầu/cuối (map sang marginPagesDisplayed)
 *  className?: string
 */
export default function Pagination({
  total,
  page,
  pageSize,
  onPageChange,
  pageSizeOptions,
  onPageSizeChange,
  showSummary = true,
  showFirstLast = true,
  siblingCount = 2,
  boundaryCount = 1,
  className = ''
}) {
  const pageCount = useMemo(
    () => Math.max(1, Math.ceil(total / pageSize)),
    [total, pageSize]
  );

  if (total === 0) {
    return showSummary ? (
      <div className={`text-xs text-slate-500 ${className}`}>No items</div>
    ) : null;
  }

  const startItem = (page - 1) * pageSize + 1;
  const endItem = Math.min(page * pageSize, total);

  const handlePage = (sel) => {
    const next = sel.selected + 1; // convert 0-based -> 1-based
    if (next !== page) onPageChange && onPageChange(next);
  };

  const gotoFirst = () => onPageChange && onPageChange(1);
  const gotoLast = () => onPageChange && onPageChange(pageCount);

  return (
    <div className={`flex flex-col gap-3 ${className}`}>
      {showSummary && (
        <div className="text-xs text-slate-600">
          Showing {startItem}-{endItem} of {total} (Page {page}/{pageCount})
        </div>
      )}

      <div className="flex items-center justify-between flex-wrap gap-3">
        <div className="flex items-center gap-2">
          {showFirstLast && (
            <button
              onClick={gotoFirst}
              disabled={page === 1}
              className="px-3 h-9 rounded-md border text-sm text-slate-600 disabled:opacity-40 disabled:cursor-not-allowed hover:bg-sky-50"
            >
              «
            </button>
          )}

            <ReactPaginate
              breakLabel="…"
              nextLabel="›"
              previousLabel="‹"
              forcePage={page - 1}
              onPageChange={handlePage}
              pageCount={pageCount}
              pageRangeDisplayed={siblingCount}
              marginPagesDisplayed={boundaryCount}
              containerClassName="flex items-center gap-2"
              pageClassName="group"
              pageLinkClassName="w-9 h-9 flex items-center justify-center rounded-md border border-slate-200 text-slate-600 hover:bg-sky-50 text-sm font-medium transition"
              activeLinkClassName="!bg-sky-500 !border-sky-500 !text-white"
              previousClassName="group"
              previousLinkClassName="px-3 h-9 flex items-center rounded-md border border-slate-200 text-slate-600 hover:bg-sky-50 disabled:opacity-40 disabled:cursor-not-allowed"
              nextClassName="group"
              nextLinkClassName="px-3 h-9 flex items-center rounded-md border border-slate-200 text-slate-600 hover:bg-sky-50 disabled:opacity-40 disabled:cursor-not-allowed"
              breakClassName="px-2 text-slate-400 select-none"
              disabledClassName="opacity-40 cursor-not-allowed"
            />

          {showFirstLast && (
            <button
              onClick={gotoLast}
              disabled={page === pageCount}
              className="px-3 h-9 rounded-md border text-sm text-slate-600 disabled:opacity-40 disabled:cursor-not-allowed hover:bg-sky-50"
            >
              »
            </button>
          )}
        </div>

        {onPageSizeChange && pageSizeOptions && (
          <div className="flex items-center gap-2 text-xs">
            <span className="text-slate-500">Per page</span>
            <select
              value={pageSize}
              onChange={(e) => {
                onPageSizeChange(Number(e.target.value));
                onPageChange && onPageChange(1);
              }}
              className="border rounded-md px-2 py-1 text-xs"
            >
              {pageSizeOptions.map(opt => (
                <option key={opt} value={opt}>{opt}</option>
              ))}
            </select>
          </div>
        )}
      </div>
    </div>
  );
}