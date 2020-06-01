import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import {WeatherForecastService} from '../api/generated/api/weatherForecast.service'

@Component({
  selector: 'app-resource-request',
  templateUrl: './resource-request.component.html',
  styleUrls: ['./resource-request.component.scss']
})
export class ResourceRequestComponent implements OnInit {

  constructor(private fb: FormBuilder, private weatherForecastService: WeatherForecastService) { }

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
    whenRequired: [''],
    mission: [''],
    fuel: [''],
    meals: [''],
    operators: [''],
    water: [''],    
    maintenance: [''],
    lodging: [''],
    power: [''],
    location: [''],
    apt: [''],
    streetAddress: [''],
    city: [''],
    province: [''],
    postcode: [''],
    additionalInformation: ['']
  });
  
  ngOnInit(): void {
  }

  onSubmit() {
    // console.log(this.resourceRequestForm.get('dateOfRequest'));
    console.log(this.resourceRequestForm.value);
  }

}
