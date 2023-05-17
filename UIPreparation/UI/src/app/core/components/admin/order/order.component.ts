import { Component, AfterViewInit, OnInit, ViewChild } from "@angular/core";
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { AlertifyService } from "app/core/services/alertify.service";
import { AuthService } from "app/core/components/admin/login/services/auth.service";
import { Observable } from "rxjs";
import { map, startWith } from "rxjs/operators";
import { LookUp } from "app/core/models/lookUp";
import { Order } from "./models/order";
import { Product } from "../product/models/product";
import { Customer } from "../customer/models/customer";
import { OrderService } from "./services/order.service";
import { WarehouseService } from "../warehouse/services/warehouse.service";
import { CustomerService } from "../customer/services/customer.service";
import { ProductService } from "../product/services/product.service";
import { LookUpService } from "app/core/services/lookUp.service";
import { OrderDto } from "./models/orderDto";
import { element } from "protractor";
import { Size } from "../product/models/size-enum";
import { Color } from "../product/models/color-enum";

declare var jQuery: any;

declare var jQuery: any;

@Component({
  selector: "app-order",
  templateUrl: "./order.component.html",
  styleUrls: ["./order.component.scss"],
})
export class OrderComponent implements AfterViewInit, OnInit {
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  productlookUp: Product[] = [];
  customerlookUp: Customer[] = [];
  displayedColumns: string[] = [
    "id",
    "createdDate",
    "lastUpdatedDate",
    "customerName",
    "productName",
    "color",
    "quantity",
    "size",
    "update",
    "delete",
  ];

  orderList: Order[];
  order: Order = new Order();

  orderDtoList: OrderDto[];
  orderDto: OrderDto = new OrderDto();

  myProductControl = new FormControl("");
  myCustomerControl = new FormControl("");

  orderAddForm: FormGroup;

  orderId: number;

  size = Size;
  sizeEnumKeys = [];

  color = Color;
  colorEnumKeys = [];

  //auto complete
  filteredProducts: Observable<Product[]>;
  filteredCustomers: Observable<Customer[]>;

  range = new FormGroup({
    start : new FormControl(),
    end : new FormControl()
  })

  startDate : string;
  endDate :string;

  constructor(
    private orderService: OrderService,
    private warehauseService: WarehouseService,
    private customerService: CustomerService,
    private productService: ProductService,
    private lookupService: LookUpService,
    private alertifyService: AlertifyService,
    private formBuilder: FormBuilder,
    private authService: AuthService
  ) {
    this.sizeEnumKeys = Object.keys(this.size);
    this.colorEnumKeys = Object.keys(this.color);
  }

  ngAfterViewInit(): void {
    this.getOrderListDto();
  }

  ngOnInit() {
    this.createOrderAddForm();
    this.getCustumerList();
    this.getProductList();
  }

  getOrderListDto() {
    this.orderService.getOrderListDto().subscribe((data) => {
      this.orderList = data;
      this.dataSource = new MatTableDataSource(data);
      this.configDataTable();
    });
  }

  save() {
    if (this.orderAddForm.valid) {
      this.order = Object.assign({}, this.orderAddForm.value);
      this.order.createdUserId = Number(this.authService.getCurrentUserId());
      this.order.lastUpdatedUserId = Number(
        this.authService.getCurrentUserId()
      );
      this.order.productId = this.myProductControl.value.id;
      this.order.customerId = this.myCustomerControl.value.id;

      if (this.order.id == 0) this.addOrder();
      else this.updateOrder();
    }
  }

  addOrder() {
    this.orderService.addOrder(this.order).subscribe(
      (data) => {
        console.log(data);
        this.getOrderListDto();
        this.order = new Order();
        jQuery("#order").modal("hide");
        this.alertifyService.success(data);
        this.clearFormGroup(this.orderAddForm);
      },
      (error) => {
        console.log(error);
        this.alertifyService.error(error.error);
      }
    );
  }

  updateOrder() {
    this.orderService.updateOrder(this.order).subscribe((data) => {
      this.orderList[index] = this.orderDto;
      var index = this.dataSource.data.findIndex((x) => x.id == this.order.id);

      this.dataSource.data[index].customerName = this.myCustomerControl.value.firstName +" "+this.myCustomerControl.value.lastName;
      this.dataSource.data[index].productName =this.myProductControl.value.productName;
      this.dataSource.data[index].size = this.myProductControl.value.size;
      this.dataSource.data[index].quantity = this.orderAddForm.value.quantity;

      this.dataSource = new MatTableDataSource(this.orderList);
      this.configDataTable();
      this.getOrderListDto();
      this.order = new Order();
      jQuery("#order").modal("hide");
      this.alertifyService.success(data);
      this.clearFormGroup(this.orderAddForm);
    });
  }

  createOrderAddForm() {
    this.orderAddForm = this.formBuilder.group({
      id: [0],
      createdUserId: [0],
      createdDate: [0],
      lastUpdatedUserId: [0],
      lastUpdatedDate: [0],
      status: [true],
      customerId: [0, Validators.required],
      customerName: [""],
      productId: [0, Validators.required],
      productName: [""],
      quantity: [0, Validators.required],
      size: [""],
    });
  }

  deleteOrder(orderId: number) {
    this.orderService.deleteOrder(orderId).subscribe((data) => {
      this.alertifyService.success(data.toString());
      this.orderList = this.orderList.filter((x) => x.id != orderId);
      this.dataSource = new MatTableDataSource(this.orderList);
      this.configDataTable();
    });
  }

  getOrderById(element: any, orderId: number) {
    this.clearFormGroup(this.orderAddForm);
    this.myCustomerControl.setValue(
      this.customerlookUp.find((y) => y.id == element.customerId)
    );
    this.myProductControl.setValue(
      this.productlookUp.find((x) => x.id == element.productId)
    );

    this.orderAddForm.controls.customerName.setValue(element.customerName);
    this.orderAddForm.controls.productName.setValue(element.productName);
    this.orderAddForm.controls.size.setValue(element.size);
    this.orderAddForm.controls.quantity.setValue(element.quantity);
    this.orderAddForm.controls.id.setValue(element.id);
  }

  clearFormGroup(group: FormGroup) {
    group.markAsUntouched();
    group.reset();

    Object.keys(group.controls).forEach((key) => {
      group.get(key).setErrors(null);
      if (key == "id") group.get(key).setValue(0);
      else if (key == "status") group.get(key).setValue(true);
    });
    this.myCustomerControl.setValue("");
    this.myProductControl.setValue("");
  }

  checkClaim(claim: string): boolean {
    return this.authService.claimGuard(claim);
  }

  configDataTable(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  applyDateFilter() : void {
    this.startDate = this.range.controls.start.value.toLocaleString("tr-TR").slice(0,10);
    this.endDate = this.range.controls.end.value.toLocaleString("tr-TR").slice(0,10);

    this.orderService.getOrderListDtoByDate(this.startDate, this.endDate).subscribe(data => {
      this.orderDtoList = data;
      this.dataSource = new MatTableDataSource(this.orderDtoList);
      this.configDataTable();
    })
  }

  clearDateFilter(){
    this.clearFormGroup(this.range);
    this.getOrderListDto();
  }

  getCustumerList() {
    this.customerService.getCustomerList().subscribe((data) => {
      this.customerlookUp = data;

      this.filteredCustomers = this.myCustomerControl.valueChanges.pipe(
        //this.orderAddForm.controls.customerId.valueChanges.pipe(
        startWith(""),
        map((value) => (value === "string" ? value : value.customerName)),
        map((name) =>
          name ? this._filter1(name) : this.customerlookUp.slice()
        )
      );
    });
  }

  //customer auto complete
  private _filter1(value: string): Customer[] {
    const filterValue = value.toLowerCase();

    return this.productlookUp.filter((option) =>
      option.productName.toLowerCase().includes(filterValue)
    );
  }

  displayFn1(customer: Customer): string {
    return customer && customer.firstName + " " + customer.lastName
      ? customer.firstName + " " + customer.lastName
      : "";
  }

  getProductList() {
    this.productService.getProductList().subscribe((data) => {
      this.productlookUp = data;

      this.filteredProducts = this.myProductControl.valueChanges.pipe(
        //this.orderAddForm.controls.productId.valueChanges.pipe(
        startWith(""),
        map((value) => (typeof value === "string" ? value : value.productName)),
        map((name) => (name ? this._filter(name) : this.productlookUp.slice()))
      );
    });
  }

  //product auto complete
  private _filter(value: string): Product[] {
    const filterValue = value.toLowerCase();

    return this.productlookUp.filter((option) =>
      option.productName.toLowerCase().includes(filterValue)
    );
  }

  displayFn(product: Product): string {
    return product && product.productName ? product.productName : "";
  }
}
