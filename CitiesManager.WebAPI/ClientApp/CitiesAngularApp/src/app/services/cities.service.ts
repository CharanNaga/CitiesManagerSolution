import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http'
const API_BASE_URL: string = "https://localhost:7283/api/";

@Injectable({
  providedIn: 'root'
})
export class CitiesService {
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

  public putCity(city: City): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", "Bearer myToken");
    return this.httpClient.put<string>(`${API_BASE_URL}v1/cities/${city.cityID}`, city, { headers: headers });
  }

  public deleteCity(cityID: string|null): Observable<string> {
    let headers = new HttpHeaders();
    headers = headers.append("Authorization", "Bearer myToken");
    return this.httpClient.delete<string>(`${API_BASE_URL}v1/cities/${cityID}`, { headers: headers });
  }
}
