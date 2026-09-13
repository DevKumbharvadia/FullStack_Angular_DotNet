import { Routes } from '@angular/router';
import { ProfileComponent } from './header/profile/profile.component';
import { CartComponent } from './header/cart/cart.component';
import { LoginComponent } from './header/login/login.component';
import { AddItemComponent } from './header/add-item/add-item.component';
import { SignupComponent } from './header/signup/signup.component';
import { StatsComponent } from './header/stats/stats.component';
import { MainComponent } from './main/main.component';
import { EditItemComponent } from './header/edit-item/edit-item.component';

export const routes: Routes = [
    {
        path:'Profile',
        component: ProfileComponent,
    },
    {
        path:'Cart',
        component:CartComponent
    },
    {
        path:'Login',
        component:LoginComponent
    },
    {
        path:'AddItem',
        component:AddItemComponent
    },
    {
        path:'SignUp',
        component:SignupComponent
    },
    {
        path:'Stats',
        component:StatsComponent
    },
    {
        path:'EditItem',
        component:EditItemComponent
    },
    {
        path:'Home',
        component:MainComponent
    }
];
