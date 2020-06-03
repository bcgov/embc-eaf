import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import {WeatherForecastService} from '../api/generated/api/weatherForecast.service'

@Component({
  selector: 'app-resource-request',
  templateUrl: './resource-request.component.html',
  styleUrls: ['./resource-request.component.scss']
})
export class ResourceRequestComponent implements OnInit {

  constructor(private fb: FormBuilder, private weatherForecastService: WeatherForecastService) { }

  resourceRequestForm = this.fb.group({
    dateOfRequest: [null],
    timeOfRequest: [null],
    trackingNumber: [null],
    priority: [null],
    requestorsCommunity: [null],
    requestorsContactInfo: [null],
    resourceCategory: [null],
    resourceType: [null],
    quantity: [null],
    unitsOfMeasure: [null],
    whenRequired: [null],
    mission: [null],
    fuel: [null],
    meals: [null],
    operators: [null],
    water: [null],    
    maintenance: [null],
    lodging: [null],
    power: [null],
    location: [null],
    apt: [null],
    streetAddress: [null],
    city: [null],
    province: [null],
    postcode: [null],
    additionalInformation: [null]
  });
  
  ngOnInit(): void {
  }

  get dateOfRequest() { return this.resourceRequestForm.get('dateOfRequest'); }

  onSubmit() {
    // console.log(this.resourceRequestForm.get('dateOfRequest'));
    console.log(this.resourceRequestForm.value);
  }

}
