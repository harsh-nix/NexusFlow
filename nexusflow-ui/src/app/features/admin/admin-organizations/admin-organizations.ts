import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { OrganizationService } from '../../../core/services/organization.service';
import { UserService } from '../../../core/services/user.service';
import { Department, Organization, Team } from '../../../core/models/organization.models';
import { AppUser } from '../../../core/models/user.models';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';

type View = 'orgs' | 'departments' | 'teams';

@Component({
  selector: 'app-admin-organizations',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatDialogModule,
  ],
  templateUrl: './admin-organizations.html',
  styleUrl: './admin-organizations.css',
})
export class AdminOrganizationsComponent implements OnInit {
  view = signal<View>('orgs');

  organizations = signal<Organization[]>([]);
  departments = signal<Department[]>([]);
  teams = signal<Team[]>([]);
  allUsers = signal<AppUser[]>([]);

  selectedOrg = signal<Organization | null>(null);
  selectedDept = signal<Department | null>(null);

  isLoading = signal(true);
  isSaving = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  showOrgForm = signal(false);
  showDeptForm = signal(false);
  showTeamForm = signal(false);
  addingMemberToTeamId = signal<number | null>(null);

  orgForm: FormGroup;
  deptForm: FormGroup;
  teamForm: FormGroup;
  memberForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private organizationService: OrganizationService,
    private userService: UserService,
    private dialog: MatDialog
  ) {
    this.orgForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      website: [''],
    });

    this.deptForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });

    this.teamForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
    });

    this.memberForm = this.fb.group({
      userId: [null, Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadOrganizations();
  }

  // ---------- Organizations ----------

  loadOrganizations(): void {
    this.isLoading.set(true);
    this.organizationService.getAllOrganizations().subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) this.organizations.set(res.data);
        else this.errorMessage.set(res.message);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  toggleOrgForm(): void {
    this.showOrgForm.set(!this.showOrgForm());
    this.orgForm.reset();
  }

  submitOrg(): void {
    if (this.orgForm.invalid) {
      this.orgForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.clearMessages();

    this.organizationService.createOrganization(this.orgForm.value).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.successMessage.set('Organization created.');
          this.showOrgForm.set(false);
          this.orgForm.reset();
          this.loadOrganizations();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  deleteOrg(org: Organization): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete organization?',
        message: `"${org.name}" and its department/team structure reference will be removed.`,
        confirmText: 'Delete',
        destructive: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;
      this.organizationService.deleteOrganization(org.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.successMessage.set('Organization deleted.');
            this.loadOrganizations();
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: (err) => this.errorMessage.set(this.extractError(err)),
      });
    });
  }

  openDepartments(org: Organization): void {
    this.selectedOrg.set(org);
    this.view.set('departments');
    this.loadDepartments();
  }

  // ---------- Departments ----------

  loadDepartments(): void {
    const org = this.selectedOrg();
    if (!org) return;

    this.isLoading.set(true);
    this.organizationService.getDepartments(org.id).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) this.departments.set(res.data);
        else this.errorMessage.set(res.message);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  toggleDeptForm(): void {
    this.showDeptForm.set(!this.showDeptForm());
    this.deptForm.reset();
  }

  submitDept(): void {
    const org = this.selectedOrg();
    if (!org || this.deptForm.invalid) {
      this.deptForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.clearMessages();

    this.organizationService.createDepartment(org.id, this.deptForm.value).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.successMessage.set('Department created.');
          this.showDeptForm.set(false);
          this.deptForm.reset();
          this.loadDepartments();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  deleteDept(dept: Department): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete department?',
        message: `"${dept.name}" and its teams will be removed.`,
        confirmText: 'Delete',
        destructive: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;
      this.organizationService.deleteDepartment(dept.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.successMessage.set('Department deleted.');
            this.loadDepartments();
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: (err) => this.errorMessage.set(this.extractError(err)),
      });
    });
  }

  openTeams(dept: Department): void {
    this.selectedDept.set(dept);
    this.view.set('teams');
    this.loadTeams();
    this.loadAllUsers();
  }

  backToOrgs(): void {
    this.view.set('orgs');
    this.selectedOrg.set(null);
  }

  backToDepartments(): void {
    this.view.set('departments');
    this.selectedDept.set(null);
  }

  // ---------- Teams ----------

  loadTeams(): void {
    const dept = this.selectedDept();
    if (!dept) return;

    this.isLoading.set(true);
    this.organizationService.getTeams(dept.id).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        if (res.success) this.teams.set(res.data);
        else this.errorMessage.set(res.message);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  loadAllUsers(): void {
    this.userService.getAll().subscribe({
      next: (res) => {
        if (res.success) this.allUsers.set(res.data);
      },
    });
  }

  toggleTeamForm(): void {
    this.showTeamForm.set(!this.showTeamForm());
    this.teamForm.reset();
  }

  submitTeam(): void {
    const dept = this.selectedDept();
    if (!dept || this.teamForm.invalid) {
      this.teamForm.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.clearMessages();

    this.organizationService.createTeam(dept.id, this.teamForm.value).subscribe({
      next: (res) => {
        this.isSaving.set(false);
        if (res.success) {
          this.successMessage.set('Team created.');
          this.showTeamForm.set(false);
          this.teamForm.reset();
          this.loadTeams();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(this.extractError(err));
      },
    });
  }

  deleteTeam(team: Team): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete team?',
        message: `"${team.name}" and its member list will be removed.`,
        confirmText: 'Delete',
        destructive: true,
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (!confirmed) return;
      this.organizationService.deleteTeam(team.id).subscribe({
        next: (res) => {
          if (res.success) {
            this.successMessage.set('Team deleted.');
            this.loadTeams();
          } else {
            this.errorMessage.set(res.message);
          }
        },
        error: (err) => this.errorMessage.set(this.extractError(err)),
      });
    });
  }

  toggleAddMember(team: Team): void {
    this.addingMemberToTeamId.set(this.addingMemberToTeamId() === team.id ? null : team.id);
    this.memberForm.reset();
  }

  submitAddMember(teamId: number): void {
    if (this.memberForm.invalid) {
      this.memberForm.markAllAsTouched();
      return;
    }

    this.clearMessages();

    this.organizationService.addTeamMember(teamId, this.memberForm.value).subscribe({
      next: (res) => {
        if (res.success) {
          this.successMessage.set('Member added.');
          this.addingMemberToTeamId.set(null);
          this.loadTeams();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => this.errorMessage.set(this.extractError(err)),
    });
  }

  removeMember(memberId: number): void {
    this.organizationService.removeTeamMember(memberId).subscribe({
      next: (res) => {
        if (res.success) {
          this.successMessage.set('Member removed.');
          this.loadTeams();
        } else {
          this.errorMessage.set(res.message);
        }
      },
      error: (err) => this.errorMessage.set(this.extractError(err)),
    });
  }

  private clearMessages(): void {
    this.errorMessage.set(null);
    this.successMessage.set(null);
  }

  private extractError(err: any): string {
    const apiErrors = err.error?.errors as string[] | undefined;
    return apiErrors?.join(' ') || err.error?.message || 'Something went wrong.';
  }
}