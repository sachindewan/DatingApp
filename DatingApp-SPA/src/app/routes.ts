import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

import { HomeComponent } from './home/home.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { AuthGuard } from './_guards/auth.guard';
import { AuthService } from './_services/auth.service';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolver/member-detail-resolver';
import { MemberListResolver } from './_resolver/member-list-resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolver/member-edit-resolver';
import { PreventUnSavedChages } from './_guards/prevent-unsaved-changes.guard';
import { ListResolver } from './_resolver/list-resolver';
import { MessageResolver } from './_resolver/message-resolver';

export const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
  },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'lists',
        component: ListsComponent,
        resolve: { users: ListResolver },
      },
      {
        path: 'members',
        component: MemberListComponent,
        resolve: { users: MemberListResolver },
      },
      {
        path: 'member/:id',
        component: MemberDetailComponent,
        resolve: { user: MemberDetailResolver },
      },
      {
        path: 'memberProfile/edit',
        component: MemberEditComponent,
        resolve: { user: MemberEditResolver },
        canDeactivate: [PreventUnSavedChages],
      },
      {
        path: 'messages',
        component: MessagesComponent,
        resolve: { messages: MessageResolver },
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
    pathMatch: 'full',
  },
];
@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      enableTracing: false, // <-- debugging purposes only
    }),
  ],
  exports: [RouterModule],
  providers: [],
})
export class RoutesModule {}
