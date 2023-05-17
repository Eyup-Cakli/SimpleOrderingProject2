﻿import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Order } from '../models/order';
import { environment } from 'environments/environment';



@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private httpClient: HttpClient) { }

  getOrderListDtoByDate(startDate :string, endDate :string) : Observable<Order[]>{
    const params ={startDate, endDate}
    return this.httpClient.get<Order[]>(environment.getApiUrl + '/orders/dtosbydate', {params});
  }

  getOrderListDto(): Observable<Order[]> {

    return this.httpClient.get<Order[]>(environment.getApiUrl + '/orders/dtos')
  }

  getOrderById(id: number): Observable<Order> {
    return this.httpClient.get<Order>(environment.getApiUrl + '/orders/getbyid?id='+id)
  }

  addOrder(order: Order): Observable<any> {

    return this.httpClient.post(environment.getApiUrl + '/orders/', order, { responseType: 'text' });
  }

  updateOrder(order: Order): Observable<any> {
    return this.httpClient.put(environment.getApiUrl + '/orders/', order, { responseType: 'text' });

  }

  deleteOrder(id: number) {
    return this.httpClient.request('delete', environment.getApiUrl + '/orders/', { body: { id: id } });
  }


}