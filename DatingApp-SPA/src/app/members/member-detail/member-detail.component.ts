import { Component, OnInit, ViewChild } from '@angular/core';
import {
  NgxGalleryOptions,
  NgxGalleryImage,
  NgxGalleryAnimation,
} from 'ngx-gallery-9';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { User } from 'src/app/_models/user';
import { TabsetComponent } from 'ngx-bootstrap/tabs/public_api';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  user: User;
  @ViewChild('memberTabs', { static: true }) memberTabs: TabsetComponent;
  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.user = data.user;
    });
    this.route.queryParams.subscribe((params) => {
      const selectedTab = params['tab'];
      this.memberTabs.tabs[selectedTab > 0 ? selectedTab : 0].active = true;
    });
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];
    this.galleryImages = this.getImages();
  }
  getImages() {
    const imagesUrls = [];
    for (const photo of this.user.photos) {
      imagesUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description,
      });
    }
    return imagesUrls;
  }
  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }
}
