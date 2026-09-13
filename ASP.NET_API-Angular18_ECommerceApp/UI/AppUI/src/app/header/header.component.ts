import { NgClass } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [NgClass, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  isDropdownVisible = false;

  isAdmin: boolean = true;
  isLogin: boolean = true;

  toggleDropdown() {
    this.isDropdownVisible = !this.isDropdownVisible;
  }
}