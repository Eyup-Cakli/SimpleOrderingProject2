import { Injectable } from '@angular/core';
import { Order } from '../modules/order';
import { HttpEntityRepositoryService } from 'app/core/services/http-entity-repository.service';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private service: HttpEntityRepositoryService<any>,
    private readonly _httpClient: HttpClient) { }

  getOrderList(): Observable<Order[]> {
    return this._httpClient.get<Order[]>(environment.getApiUrl +'/orders/');
  }

  getOrder(id: number): Observable<Order> {
    return this._httpClient.get<Order>(environment.getApiUrl +`/orders/${id}`);
  }

  addOrder(order: Order): Observable<any> {
    return this._httpClient.post(environment.getApiUrl +"/orders/", order, { responseType: 'text' });
  }

  updateOrder(order: Order): Observable<any> {
    return this._httpClient.put(environment.getApiUrl +"/orders/", order, { responseType: 'text' });
  }

  deleteOrder(id: number) {
    return this._httpClient.delete(environment.getApiUrl +`/orders/${id}`);
  }
}
