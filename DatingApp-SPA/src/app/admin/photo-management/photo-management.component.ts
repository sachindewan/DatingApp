import { Component, OnInit } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css'],
})
export class PhotoManagementComponent implements OnInit {
  photos: Photo[] = [];
  constructor(
    private userService: UserService,
    private alertifyService: AlertifyService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.userService
      .getPendingForApprovalPhoto(this.authService.decodedToken.nameid)
      .subscribe(
        (res: Photo[]) => {
          this.photos = res;
        },
        (error) => this.alertifyService.error(error)
      );
  }

  approvePhoto(photo: Photo) {
    this.userService
      .approvePhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          this.photos.splice(
            this.photos.findIndex((x) => x.id === photo.id),
            1
          );
        },
        (error) => this.alertifyService.error(error)
      );
  }
}
