import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UrlTable } from './url-table';

describe('UrlTable', () => {
  let component: UrlTable;
  let fixture: ComponentFixture<UrlTable>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UrlTable],
    }).compileComponents();

    fixture = TestBed.createComponent(UrlTable);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
