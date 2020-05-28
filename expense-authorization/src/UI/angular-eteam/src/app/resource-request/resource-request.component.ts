import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import {WeatherForecastService} from 'build/openapi/api/weatherForecast.service'

@Component({
  selector: 'app-resource-request',
  templateUrl: './resource-request.component.html',
  styleUrls: ['./resource-request.component.scss']
})
export class ResourceRequestComponent implements OnInit {

  constructor(private fb: FormBuilder, private weatherForecastService: WeatherForecastService) { }

  weatherArticles: any;

  resourceRequestForm = this.fb.group({
    dateOfRequest: [''],
    timeOfRequest: [''],
    trackingNumber: [''],
    priority: [''],
    requestorsCommunity: [''],
    requestorsContactInfo: [''],
    resourceCategory: [''],
    resourceType: [''],
    quantity: [''],
    unitsOfMeasure: [''],
    whenRequired: ['']
  });
  
  ngOnInit(): void {
    this.weatherArticles = this.weatherForecastService.weatherForecastGet();
  }

  onSubmit() {
    // console.log(this.resourceRequestForm.get('dateOfRequest'));
    console.log(this.resourceRequestForm.value);
  }

}
