import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http'

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
    return this.httpClient.get<City[]>("https://localhost:7283/api/v1/cities");
  }
}
