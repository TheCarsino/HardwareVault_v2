// types/manufacturer.ts
export interface Manufacturer {
  manufacturerId: number;
  name: string;
  type: 'CPU' | 'GPU' | 'Both';
  website?: string;
}
