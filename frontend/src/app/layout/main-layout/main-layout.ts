import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar} from '../sidebar/sidebar';
import { Topbar } from '../topbar/topbar';
import { LayoutService } from '../../core/services/layout.service';

@Component({
  selector: 'app-main-layout',
  imports: [RouterOutlet, Sidebar, Topbar],
  templateUrl: './main-layout.html',
  styleUrl: './main-layout.scss',
})
export class MainLayout {

  constructor(public layoutService: LayoutService) {}

  get isCollapsed(): boolean {
    return this.layoutService.isCollapsed();
  }

}
