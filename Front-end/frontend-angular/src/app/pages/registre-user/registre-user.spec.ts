import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RegistreUser } from './registre-user';

describe('RegistreUser', () => {
  let component: RegistreUser;
  let fixture: ComponentFixture<RegistreUser>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegistreUser],
    }).compileComponents();

    fixture = TestBed.createComponent(RegistreUser);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
