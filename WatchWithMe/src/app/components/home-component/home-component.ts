import { Component } from '@angular/core';
import { WorkStep } from '@models/work-step';

@Component({
  selector: 'app-home-component',
  imports: [],
  templateUrl: './home-component.html',
  styleUrl: './home-component.css',
})
export class HomeComponent {

  // The index of the currently selected step (0 means the first step)
  activeStepIndex: number = 0;

  // Our array of steps with dummy data matching your cat/movie theme
  steps: WorkStep[] = [
    {
      id: 1,
      title: 'Create a Room',
      description: 'Click a single button to spawn a new room. You will get a unique link instantly.',
      videoUrl: '/assets/videos/demo-app-vid.mp4'
    },
    {
      id: 2,
      title: 'Invite the Cats (Friends)',
      description: 'Send the link or code to your friends. They can join as guests in one click.',
      videoUrl: '/assets/videos/demo-app-vid.mp4'
    },
    {
      id: 3,
      title: 'Press Play & Sync',
      description: 'Choose what to watch. The video will automatically sync for everyone in the room.',
      videoUrl: '/assets/videos/demo-app-vid.mp4'
    }
  ];

  // Function called when a user clicks a step
  selectStep(index: number): void {
    this.activeStepIndex = index;
  }

}
