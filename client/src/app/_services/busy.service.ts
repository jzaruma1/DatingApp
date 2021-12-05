import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
  busyRequestTime = 0;

  constructor(private spinnerService: NgxSpinnerService) { }

  busy() {
    this.busyRequestTime++;
    this.spinnerService.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgba(255,255,255,0)',
      color: "#333333"
    });
  }

  idle() {
    this.busyRequestTime--;
    if (this.busyRequestTime <= 0) {
      this.busyRequestTime = 0;
      this.spinnerService.hide();
    }
  }
}
