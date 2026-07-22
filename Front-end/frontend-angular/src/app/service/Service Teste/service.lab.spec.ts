import { TestBed } from '@angular/core/testing';

import { ServiceLab } from './service.lab';

describe('ServiceLab', () => {
  let service: ServiceLab;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ServiceLab);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
