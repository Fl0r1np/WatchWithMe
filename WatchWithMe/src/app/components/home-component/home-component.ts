import { Component } from '@angular/core';
import { WorkStep } from '@models/work-step';

@Component({
  selector: 'app-home-component',
  imports: [],
  template: `
    <main class="home-page">
      <section class="hero">
        
        <div class="hero-content">
          <h1 class="hero-title">
            Watch movies together with your <span class="hero-title-highlight">friends</span>
          </h1>
          <p class="hero-description">
            Create a room, invite your friends, and enjoy your favorite shows in perfect sync. Grab your popcorn and let the cats handle the rest!
          </p>
          
          <div class="hero-actions">
            <button class="btn btn--primary">Create Room</button>
            <button class="btn btn--secondary">Join Room</button>
          </div>
        </div>

        <div class="hero-visual">
          <video class="hero-video" [autoplay]="true" [loop]="true" [muted]="true" playsinline poster="/assets/hero/demo-poster.png">
            <source src="/assets/videos/demo-app-vid.mp4" type="video/mp4">
            Your browser does not support the video tag.
          </video>
        </div>

      </section>

      <div class="features-container">

        <section class="feature-block">
          <div class="feature-block-visual">
            <img src="assets/hero/demo-poster.png" alt="Cats watching and reacting in sync" class="feature-block-image">
          </div>
          <div class="feature-block-content">
            <h2 class="feature-block-title">In Perfect <span class="text-highlight">Sync</span></h2>
            <p class="feature-block-description">
              Everyone sees and reacts to the exact same thing at the exact same time. Plus, anyone in the room has the power to navigate, pause, or change the content on the fly. No more counting down to press play!
            </p>
          </div>
        </section>

        <section class="feature-block feature-block--reverse">
          <div class="feature-block-visual">
            <img src="assets/hero/demo-poster.png" alt="Relaxed cat enjoying a free, ad-less experience" class="feature-block-image">
          </div>
          <div class="feature-block-content">
            <h2 class="feature-block-title">100% Free & <span class="text-highlight">Ad-Free</span></h2>
            <p class="feature-block-description">
              We believe watch parties should be easy and accessible. Enjoy a seamless, user-friendly experience completely free of charge, with zero annoying ads to interrupt your movie night.
            </p>
          </div>
        </section>

        <section class="feature-block">
          <div class="feature-block-visual">
            <img src="assets/hero/demo-poster.png" alt="Cat holding an invite link" class="feature-block-image">
          </div>
          <div class="feature-block-content">
            <h2 class="feature-block-title">Join Instantly, <span class="text-highlight">No Account Needed</span></h2>
            <p class="feature-block-description">
              Jumping into a room is as easy as a single click. Your friends don't need to create an account to participate as guests—just share the room link or code, and they are instantly in!
            </p>
          </div>
        </section>

      </div>

      <section class="how-it-works">
  
        <div class="how-it-works-header">
          <h2 class="how-it-works-title">How does it <span class="text-highlight">work?</span></h2>
          <p class="how-it-works-subtitle">Three simple steps to start your watch party.</p>
        </div>

        <div class="how-it-works-content">
          
          <div class="how-it-works-steps">
            
            @for (step of steps; track step.id; let i = $index) {
              
              <button 
                class="step-item" 
                [class.step-item--active]="i === activeStepIndex"
                (click)="selectStep(i)">
                
                <div class="step-item-number">{{ i + 1 }}</div>
                <div class="step-item-text">
                  <h3 class="step-item-title">{{ step.title }}</h3>
                  @if (i === activeStepIndex) {
                    <p class="step-item-description">{{ step.description }}</p>
                  }
                </div>
                
              </button>
              
            }

          </div>

          <div class="how-it-works-visual">
            <video 
              class="how-it-works-video" 
              [src]="steps[activeStepIndex].videoUrl" 
              [autoplay]="true" 
              [loop]="true" 
              [muted]="true" 
              playsinline>
            </video>
          </div>

        </div>
      </section>

    </main>
  `,
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
