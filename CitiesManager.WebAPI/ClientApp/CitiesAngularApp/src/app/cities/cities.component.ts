import { Component } from '@angular/core';
import { CitiesService } from '../services/cities.service';
import { City } from '../models/city';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.css']
})
export class CitiesComponent {
  cities: City[] = [];
  constructor(private citiesService: CitiesService) {
  }
  ngOnInit() {
    this.cities = this.citiesService.getCities();
  }
}
