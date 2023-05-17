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
import { LookUpService } from "app/core/services/lookUp.service";
import { AuthService } from "app/core/components/admin/login/services/auth.service";
import { Warehouse } from "./models/warehouse";
import { WarehouseService } from "./services/warehouse.service";
import { environment } from "environments/environment";
import { LookUp } from "app/core/models/lookUp";
import { ReadyForSale } from "./models/readyForSale-enum";
import { Product } from "../product/models/product";
import { Observable } from "rxjs";
import { ProductService } from "../product/services/product.service";
import { map, startWith } from "rxjs/operators";
import { WarehouseDto } from "./models/warehouseDto";
import { data } from "jquery";
import { formatDate } from "@angular/common";
import { MatDatepickerInputEvent } from "@angular/material/datepicker";
import { Size } from "../product/models/size-enum";
import { Color } from "../product/models/color-enum";

declare var jQuery: any;

const today = new Date();

@Component({
  selector: "app-warehouse",
  templateUrl: "./warehouse.component.html",
  styleUrls: ["./warehouse.component.scss"],
})
export class WarehouseComponent implements AfterViewInit, OnInit {
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  productlookUp: Product[] = [];

  displayedColumns: String[] = [
    "id",
    "createdDate",
    "lastUpdatedDate",
    "userName",
    "productName",
    "size",
    "quantity",
    "readyForSale",
    "update",
    "delete",
  ];

  //auto complete
  filteredProducts: Observable<Product[]>;

  warehouseDtoList: WarehouseDto[] = [];
  warehouseDto: WarehouseDto = new WarehouseDto();

  warehouseList: Warehouse[] = [];
  warehouse: Warehouse = new Warehouse();

  myProductControl = new FormControl("");

  warehouseAddForm: FormGroup;

  warehouseId: number;

  readyForSale = ReadyForSale;
  readyForSaleEnumKeys = [];

  size = Size;
  sizeEnumKeys = [];

  color = Color;
  colorEnumKeys = [];

  constructor(
    private productService: ProductService,
    private warehouseService: WarehouseService,
    private lookupService: LookUpService,
    private alertifyService: AlertifyService,
    private formBuilder: FormBuilder,
    private authService: AuthService
  ) {}

  range = new FormGroup({
    start: new FormControl(),
    end: new FormControl(),
  });

  startDate: string;
  endDate: string;

  ngAfterViewInit(): void {
    this.getWarehouseListDto();
    this.readyForSaleEnumKeys = Object.keys(this.readyForSale);
    this.sizeEnumKeys = Object.keys(this.size);
    this.colorEnumKeys = Object.keys(this.color);
  }

  ngOnInit() {
    this.createWarehouseAddForm();
    this.getProductList();
  }

  getWarehouseListDto() {
    this.warehouseService.getWarehouseListDto().subscribe((data) => {
      this.warehouseList = data;
      this.dataSource = new MatTableDataSource(data);
      this.configDataTable();
    });
  }

  getWarehouseDtoListByDate() {
    this.warehouseService.getWarehouseDtoListByDate(this.startDate.toString(), this.endDate.toString()).subscribe((data) => {
      this.warehouseList = data;
      this.dataSource = new MatTableDataSource(data);
      this.configDataTable();
    });
  }

  save() {
    if (this.warehouseAddForm.valid) {
      console.log(this.myProductControl.value.id);
      console.log(this.warehouse);

      this.warehouse = Object.assign({}, this.warehouseAddForm.value);
      this.warehouse.createdUserId = Number(
        this.authService.getCurrentUserId()
      );
      this.warehouse.lastUpdatedUserId = Number(
        this.authService.getCurrentUserId()
      );
      this.warehouse.productId = this.myProductControl.value.id;

      if (this.warehouse.id == 0) {
        this.addWarehouse();
      } else this.updateWarehouse();
    }
  }

  addWarehouse() {
    this.warehouseService.addWarehouse(this.warehouse).subscribe(
      (data) => {
        this.getWarehouseListDto();
        this.warehouse = new Warehouse();
        jQuery("#warehouse").modal("hide");
        this.alertifyService.success(data);
        this.clearFormGroup(this.warehouseAddForm);
        this.warehouseAddForm.reset;
      },
      (error) => {
        console.log(error);
        this.alertifyService.error(error.error);
      }
    );
  }

  updateWarehouse() {
    this.warehouseService.updateWarehouse(this.warehouse).subscribe((data) => {
      console.log(data);
      var index = this.dataSource.data.findIndex(
        (x) => x.id == this.warehouse.id
      );
      this.warehouseList[index] = this.warehouse;

      this.dataSource.data[index].productName =
        this.myProductControl.value.productName;
      this.dataSource.data[index].quantity =
        this.warehouseAddForm.value.quantity;
      this.dataSource.data[index].readyForSale =
        this.warehouseAddForm.value.readyForSale;
        this.dataSource.data[index].size = 
        this.warehouseAddForm.value.size;
        this.dataSource.data[index].color = 
        this.warehouseAddForm.value.color;

      this.dataSource = new MatTableDataSource(this.warehouseList);
      this.configDataTable();
      this.getWarehouseListDto();
      this.warehouse = new Warehouse();
      jQuery("#warehouse").modal("hide");
      this.alertifyService.success(data);
      this.clearFormGroup(this.warehouseAddForm);
    });
  }

  createWarehouseAddForm() {
    this.warehouseAddForm = this.formBuilder.group({
      id: [0],
      createdUserId: [0],
      createdDate: [0],
      startDate: [0],
      endDate: [0],
      lastUpdatedUserId: [0],
      lastUpdatedDate: [0],
      userName: [""],
      status: [true],
      productId: [0, Validators.required],
      productName: [""],
      size: [""],
      quantity: [0, Validators.required],
      readyForSale: ["", Validators.required],
    });
  }

  deleteWarehouse(warehouseId: number) {
    this.warehouseService.deleteWarehouse(warehouseId).subscribe((data) => {
      this.alertifyService.success(data.toString());
      this.warehouseList = this.warehouseList.filter(
        (x) => x.id != warehouseId
      );
      this.dataSource = new MatTableDataSource(this.warehouseList);
      this.configDataTable();
    });
  }

  getWarehouseById(element: any) {
    this.clearFormGroup(this.warehouseAddForm);
    this.myProductControl.setValue(
      this.productlookUp.find((x) => x.id == element.productId)
    );

    this.warehouseAddForm.controls.id.setValue(element.id);
    this.warehouseAddForm.controls.createdDate.setValue(element.createdDate);
    this.warehouseAddForm.controls.lastUpdatedDate.setValue(
      element.lastUpdatedDate
    );
    this.warehouseAddForm.controls.productName.setValue(element.productName);
    this.warehouseAddForm.controls.quantity.setValue(element.quantity);
    this.warehouseAddForm.controls.readyForSale.setValue(element.readyForSale);
    this.warehouseAddForm.controls.size.setValue(element.size);
    this.warehouseAddForm.controls.color.setValue(element.color);
  }

  clearFormGroup(group: FormGroup) {
    group.markAsUntouched();
    group.reset();

    Object.keys(group.controls).forEach((key) => {
      group.get(key).setErrors(null);
      if (key == "id") group.get(key).setValue(0);
      else if (key == "status") group.get(key).setValue(true);
    });
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
    this.startDate = this.range.controls.start.value.toLocaleString('tr-TR').slice(0, 10);
    this.endDate = this.range.controls.end.value.toLocaleString('tr-TR').slice(0, 10);

    this.warehouseService.getWarehouseDtoListByDate(this.startDate, this.endDate).subscribe(data =>{
      this.warehouseDtoList =data;
      this.dataSource = new MatTableDataSource(this.warehouseDtoList);
      this.configDataTable();
    })
    console.log("Start : "+this.startDate);
    console.log("End : "+this.endDate);
  }

  clearDateFilter() {
    this.clearFormGroup(this.range);
    this.getWarehouseListDto();
  }

  getProductList() {
    this.productService.getProductList().subscribe((data) => {
      this.productlookUp = data;

      this.filteredProducts = this.myProductControl.valueChanges.pipe(
        startWith(""),
        map((value) => (typeof value === "string" ? value : value.productName)),
        map((name) => (name ? this._filter(name) : this.productlookUp.slice()))
      );
    });
  }

  //auto complete
  private _filter(value: string): Product[] {
    const filterValue = value.toLowerCase();

    return this.productlookUp.filter((option) =>
      option.productName.toLowerCase().includes(filterValue)
    );
  }

  displayFnProduct(p: Product): string {
    return p && p.productName ? p.productName : "";
  }
}
