import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http'
const API_BASE_URL: string = "https://localhost:7283/api/";

@Injectable({
  providedIn: 'root'
})
export class CitiesService {
  //cities: City[] = [];
  //constructor() {
  //  this.cities = [
  //    new City("101", "City-A"),
  //    new City("102", "City-B"),
  //    new City("103", "City-C"),
  //    new City("104", "City-D")
  //  ];
  //}
  //public getCities(): City[] {
  //  return this.cities;
  //}

  cities: City[] = [];
  constructor(private httpClient: HttpClient) {

  }
  public getCities(): Observable<City[]> {
    //adding authorization as a header
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", "Bearer myToken");
    return this.httpClient.get<City[]>(`${API_BASE_URL}v1/cities`, { headers: headers });
  }
  public postCity(city: City): Observable<City> {
    //adding authorization as a header
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", "Bearer myToken");
    return this.httpClient.post<City>(`${API_BASE_URL}v1/cities`,city, { headers: headers });
  }
}
