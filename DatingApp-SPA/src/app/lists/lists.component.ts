import { Component, OnInit } from '@angular/core';
import { Pagination, PagenatedResult } from '../_models/pagination';
import { User } from '../_models/user';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  pagination: Pagination;
  users: User[];
  likesParam: string;
  constructor(
    private authService: AuthService,
    private userService: UserService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.likesParam = 'Likers';
  }
  loadUser() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        null,
        this.likesParam
      )
      .subscribe((res: PagenatedResult<User[]>) => {
        this.users = res.result;
        this.pagination = res.pagination;
      });
  }
  pageChanged(event) {
    this.pagination.currentPage = event.page;
    this.loadUser();
  }
}
