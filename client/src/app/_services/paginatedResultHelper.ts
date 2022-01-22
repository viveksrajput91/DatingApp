import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/PaginatedResult";

export function getPaginationHeaders(pageNumber,pageSize)
{
  let httpParams=new HttpParams();
  httpParams=httpParams.append("pageNumber",pageNumber.toString());
  httpParams=httpParams.append("pageSize",pageSize.toString());
  return httpParams;
}

export function getPaginatedResult<T>(url:string,httpParams:HttpParams,http:HttpClient)
{
  const paginatedResult:PaginatedResult<T>=new PaginatedResult<T>();
1
  return http.get<T>(url,{observe:'response',params:httpParams}).pipe(
    map(response=>{
      paginatedResult.result=response.body;
      if(response.headers.get('Pagination')!== null)
      {
        paginatedResult.pagination= JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })
  );
}
