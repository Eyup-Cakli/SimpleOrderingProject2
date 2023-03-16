import { Component, AfterViewInit, OnInit, ViewChild } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import { AlertifyService } from "app/core/services/alertify.service";
import { LookUpService } from "app/core/services/lookUp.service";
import { AuthService } from "app/core/components/admin/login/services/auth.service";
import { Warehouse } from "./models/Warehouse";
import { WarehouseService } from "./services/Warehouse.service";
import { environment } from "environments/environment";

declare var jQuery: any;

@Component({
  selector: "app-warehouse",
  templateUrl: "./warehouse.component.html",
  styleUrls: ["./warehouse.component.scss"],
})
export class WarehouseComponent implements AfterViewInit, OnInit {
  dataSource: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  displayedColumns: string[] = [
    "id",
    "createdUserId",
    "createdDate",
    "lastUpdatedUserId",
    "lastUpdatedDate",
    "status",
    "isDeleted",
    "productId",
    "quantity",
    "readyForSale",
    "update",
    "delete",
  ];

  warehouseList: Warehouse[]=[];
  warehouse: Warehouse = new Warehouse();

  warehouseAddForm: FormGroup;

  warehouseId: number;

  constructor(
    private warehouseService: WarehouseService,
    private lookupService: LookUpService,
    private alertifyService: AlertifyService,
    private formBuilder: FormBuilder,
    private authService: AuthService
  ) {}

  ngAfterViewInit(): void {
    this.getWarehouseList();
  }

  ngOnInit() {
    this.createWarehouseAddForm();
  }

  getWarehouseList() {
    this.warehouseService.getWarehouseList().subscribe((data) => {
      this.warehouseList = data;
      this.dataSource = new MatTableDataSource(data);
      this.configDataTable();
    });
  }

  save() {
    if (this.warehouseAddForm.valid) {
      this.warehouse = Object.assign({}, this.warehouseAddForm.value);

      if (this.warehouse.id == 0) this.addWarehouse();
      else this.updateWarehouse();
    }
  }

  addWarehouse() {
    this.warehouseService.addWarehouse(this.warehouse).subscribe((data) => {
      this.getWarehouseList();
      this.warehouse = new Warehouse();
      jQuery("#warehouse").modal("hide");
      this.alertifyService.success(data);
      this.clearFormGroup(this.warehouseAddForm);
    });
  }

  updateWarehouse() {
    this.warehouseService.updateWarehouse(this.warehouse).subscribe((data) => {
      var index = this.warehouseList.findIndex(
        (x) => x.id == this.warehouse.id
      );
      this.warehouseList[index] = this.warehouse;
      this.dataSource = new MatTableDataSource(this.warehouseList);
      this.configDataTable();
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
      createdDate: [null],
      lastUpdatedUserId: [0],
      lastUpdatedDate: [null],
      status: [false],
      isDeleted: [false],
      productId: [0, Validators.required],
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

  getWarehouseById(warehouseId: number) {
    this.clearFormGroup(this.warehouseAddForm);
    this.warehouseService.getWarehouseById(warehouseId).subscribe((data) => {
      this.warehouse = data;
      this.warehouseAddForm.patchValue(data);
    });
  }

  clearFormGroup(group: FormGroup) {
    group.markAsUntouched();
    group.reset();

    Object.keys(group.controls).forEach((key) => {
      group.get(key).setErrors(null);
      if (key == "id") group.get(key).setValue(0);
    });
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
}
