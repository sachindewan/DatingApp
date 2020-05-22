import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { User } from '../../_models/user';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PagenatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  pagination: Pagination;
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];
  userParams: any = {};
  constructor(
    private userService: UserService,
    private alrtifyService: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.users = data.users.result;
      this.pagination = data['users'].pagination;
    });
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
  }
  resetFilters() {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.loadUser();
  }
  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.loadUser();
  }

  loadUser() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.userParams
      )
      .subscribe((res: PagenatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
      });
  }
}
