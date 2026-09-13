import { Component, inject } from '@angular/core';
import { ProfileService } from './profile.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  profileService = inject(ProfileService);

  updateHistory(){
    this.profileService.getHistory();
  }
}
