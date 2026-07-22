import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParenteData } from './parente-data';

describe('ParenteData', () => {
  let component: ParenteData;
  let fixture: ComponentFixture<ParenteData>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ParenteData],
    }).compileComponents();

    fixture = TestBed.createComponent(ParenteData);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
