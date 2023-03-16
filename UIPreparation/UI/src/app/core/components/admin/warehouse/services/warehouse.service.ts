import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Warehouse } from '../models/Warehouse';
import { environment } from 'environments/environment';


@Injectable({
  providedIn: 'root'
})
export class WarehouseService {

  constructor(private httpClient: HttpClient) { }


  getWarehouseList(): Observable<Warehouse[]> {

    return this.httpClient.get<Warehouse[]>(environment.getApiUrl + '/warehouses/getall')
  }

  getWarehouseById(id: number): Observable<Warehouse> {
    return this.httpClient.get<Warehouse>(environment.getApiUrl + '/warehouses/getbyid?id='+id)
  }

  addWarehouse(warehouse: Warehouse): Observable<any> {

    return this.httpClient.post(environment.getApiUrl + '/warehouses/', warehouse, { responseType: 'text' });
  }

  updateWarehouse(warehouse: Warehouse): Observable<any> {
    return this.httpClient.put(environment.getApiUrl + '/warehouses/', warehouse, { responseType: 'text' });

  }

  deleteWarehouse(id: number) {
    return this.httpClient.request('delete', environment.getApiUrl + '/warehouses/', { body: { id: id } });
  }


}