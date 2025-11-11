import { useState, useEffect } from 'react';
import PartnersItems from "./PartnersItems";
import Pagination from "./Pagination";

function PartnersList({ partnersItem = [], onMessageClick }) {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(6);

  // Reset page khi data thay đổi
  useEffect(() => {
    setPage(1);
  }, [partnersItem]);

  const total = partnersItem.length;
  const start = (page - 1) * pageSize;
  const pageItems = partnersItem.slice(start, start + pageSize);

  return (
    <div className="w-full">
      <div className="grid grid-cols-3 gap-8 ml-4 mb-10 w-11/12">
        {pageItems.map((item, i) => (
          <PartnersItems 
            key={start + i} 
            partnersItem={item} 
            onMessageClick={onMessageClick}
          />
        ))}
        {total === 0 && (
          <div className="col-span-full text-center text-sm text-slate-500 py-10">
            No partners match filters.
          </div>
        )}
      </div>

      {total > 0 && (
        <Pagination
          total={total}
          page={page}
          pageSize={pageSize}
          onPageChange={setPage}
          onPageSizeChange={(s) => { setPageSize(s); setPage(1); }}
          className="w-11/12 ml-4 mb-16"
        />
      )}
    </div>
  );
}
export default PartnersList;