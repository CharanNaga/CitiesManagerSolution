import { Component } from '@angular/core';
import { CitiesService } from '../services/cities.service';
import { City } from '../models/city';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-cities',
  templateUrl: './cities.component.html',
  styleUrls: ['./cities.component.css']
})
export class CitiesComponent {
  cities: City[] = [];
  postCityForm: FormGroup;
  isPostCityFormSubmitted: boolean = false;
  constructor(private citiesService: CitiesService) {
    this.postCityForm = new FormGroup({
      cityName: new FormControl(null, Validators.required)
    })
  }

  loadCities() {
    this.citiesService.getCities()
      .subscribe({
        next: (response: City[]) => { this.cities = response; },
        error: (error: any) => { console.log(error) },
        complete: () => { }
      });
  }
  ngOnInit() {
    this.loadCities();
  }
//return form control object
  get postCity_CityNameControl(): any {
    return this.postCityForm.controls['cityName'];
  }
  //This method will be called in template when user submits postCityForm
  public postCitySubmitted() {
    //Add logic here
    this.isPostCityFormSubmitted = true;

    console.log(this.postCityForm.value);

    this.citiesService.postCity(this.postCityForm.value).subscribe({
      next: (response: City) => {
        console.log(response);
        //this.loadCities();
        this.cities.push(new City(response.cityID, response.cityName));
        this.postCityForm.reset();
        this.isPostCityFormSubmitted = false; //validation message will disappear after adding
      },
      error: (error: any) => {
        console.log(error);
      },
      complete: ()=> {}
    });
  }
}
