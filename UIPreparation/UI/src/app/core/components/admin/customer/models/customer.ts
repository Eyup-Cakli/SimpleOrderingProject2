export class Customer {
  id?: number;
  createdUserId?: number;
  createdDate?: Date | any;
  lastUpdatedUserId?: number;
  lastUpdatedDate?: Date | any;
  status: boolean;
  isDeleted: boolean;
  firstName?: string;
  lastName?: string;
  customerCode?: string;
  address?: string;
  phoneNumber?: string;
  email?: string;
}
