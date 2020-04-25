import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css'],
})
export class ValueComponent implements OnInit {
  value: any;
  constructor(private httpClient: HttpClient) {}

  ngOnInit() {
    this.getValue();
  }
  public getValue() {
    this.httpClient.get('http://localhost:5000/api/values').subscribe(
      (response) => {
        this.value = response;
      },
      (error) => {
        console.error(error);
      }
    );
  }
}
