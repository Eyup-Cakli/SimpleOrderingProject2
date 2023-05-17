import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Customer } from '../models/customer';
import { environment } from 'environments/environment';
import { HttpEntityRepositoryService } from 'app/core/services/http-entity-repository.service';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private readonly httpClient: HttpClient) { }

  getCustomerList(): Observable<Customer[]> {
    return this.httpClient.get<Customer[]>(environment.getApiUrl +'/customers/getall')
  }

  getCustomerById(id: number): Observable<Customer> {
    return this.httpClient.get<Customer>(environment.getApiUrl + '/customers/getbyid?id='+id)
  }

  addCustomer(customer: Customer): Observable<any> {
    return this.httpClient.post(environment.getApiUrl + '/customers/', customer, { responseType: 'text' });
  }

  updateCustomer(customer: Customer): Observable<any> {
    return this.httpClient.put(environment.getApiUrl + '/customers/', customer, { responseType: 'text' });
  }

  deleteCustomer(id: number) {
    return this.httpClient.request('delete', environment.getApiUrl + '/customers/', { body: { id: id } });
  }
}