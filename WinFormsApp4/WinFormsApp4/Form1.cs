using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PhoneBook.Models;
using PhoneBook.Services;

namespace PhoneBook
{
    public partial class MainForm : Form
    {
        private readonly DataService _dataService = new();
        private BindingSource _bindingSource = new();

        // Элементы управления
        private DataGridView dgvContacts;
        private TextBox tbSearch;
        private TextBox tbFirstName, tbLastName, tbPhone, tbEmail;
        private Button btnAdd, btnEdit, btnDelete, btnClear;
        private Label lblSearch, lblFirstName, lblLastName, lblPhone, lblEmail;
        private Panel panelInput, panelButtons;
        private TableLayoutPanel tableLayout;

        public MainForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "📞 Телефонная книга";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 500);
            this.BackColor = Color.White;

            // Создание элементов управления

            // Заголовок поиска
            lblSearch = new Label
            {
                Text = "🔍 Поиск:",
                Location = new Point(12, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            // Поле поиска
            tbSearch = new TextBox
            {
                Location = new Point(80, 9),
                Size = new Size(300, 23),
                Font = new Font("Segoe UI", 10)
            };
            tbSearch.TextChanged += TbSearch_TextChanged;

            // DataGridView
            dgvContacts = new DataGridView
            {
                Location = new Point(12, 45),
                Size = new Size(960, 300),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                GridColor = Color.LightGray,
                Font = new Font("Segoe UI", 9),
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(248, 248, 248) }
            };
            dgvContacts.SelectionChanged += DgvContacts_SelectionChanged;

            // Поля ввода
            panelInput = new Panel
            {
                Location = new Point(12, 355),
                Size = new Size(960, 100),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Подписи
            lblFirstName = new Label { Text = "Имя:", Location = new Point(15, 15), AutoSize = true };
            lblLastName = new Label { Text = "Фамилия:", Location = new Point(230, 15), AutoSize = true };
            lblPhone = new Label { Text = "Телефон:", Location = new Point(445, 15), AutoSize = true };
            lblEmail = new Label { Text = "Email:", Location = new Point(660, 15), AutoSize = true };

            // Поля
            tbFirstName = new TextBox { Location = new Point(15, 35), Size = new Size(200, 23) };
            tbLastName = new TextBox { Location = new Point(230, 35), Size = new Size(200, 23) };
            tbPhone = new TextBox { Location = new Point(445, 35), Size = new Size(200, 23) };
            tbEmail = new TextBox { Location = new Point(660, 35), Size = new Size(200, 23) };

            panelInput.Controls.AddRange(new Control[]
            {
                lblFirstName, lblLastName, lblPhone, lblEmail,
                tbFirstName, tbLastName, tbPhone, tbEmail
            });

            // Кнопки
            panelButtons = new Panel
            {
                Location = new Point(12, 460),
                Size = new Size(960, 50),
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnAdd = CreateButton("➕ Добавить", Color.FromArgb(0, 150, 136));
            btnEdit = CreateButton("✏️ Редактировать", Color.FromArgb(33, 150, 243));
            btnDelete = CreateButton("🗑️ Удалить", Color.FromArgb(244, 67, 54));
            btnClear = CreateButton("🧹 Очистить поля", Color.FromArgb(158, 158, 158));

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += BtnClear_Click;

            // Размещение кнопок
            btnAdd.Location = new Point(15, 8);
            btnEdit.Location = new Point(150, 8);
            btnDelete.Location = new Point(285, 8);
            btnClear.Location = new Point(420, 8);

            panelButtons.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnClear });

            // Добавление всех элементов на форму
            this.Controls.AddRange(new Control[]
            {
                lblSearch, tbSearch,
                dgvContacts,
                panelInput,
                panelButtons
            });

            // Подписка на закрытие формы
            this.FormClosing += MainForm_FormClosing;
        }

        private Button CreateButton(string text, Color backColor)
        {
            return new Button
            {
                Text = text,
                Width = 120,
                Height = 34,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void LoadData()
        {
            _bindingSource.DataSource = _dataService.Contacts;
            dgvContacts.DataSource = _bindingSource;
            dgvContacts.Columns["Id"].Visible = false;
        }

        private void ClearFields()
        {
            tbFirstName.Clear();
            tbLastName.Clear();
            tbPhone.Clear();
            tbEmail.Clear();
            tbFirstName.Focus();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(tbFirstName.Text))
            {
                MessageBox.Show("Введите имя.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbFirstName.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(tbLastName.Text))
            {
                MessageBox.Show("Введите фамилию.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbLastName.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(tbPhone.Text))
            {
                MessageBox.Show("Введите номер телефона.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPhone.Focus();
                return false;
            }
            return true;
        }

        private Contact GetContactFromForm()
        {
            return new Contact
            {
                FirstName = tbFirstName.Text.Trim(),
                LastName = tbLastName.Text.Trim(),
                Phone = tbPhone.Text.Trim(),
                Email = tbEmail.Text.Trim()
            };
        }

        private void LoadContactToForm(Contact contact)
        {
            tbFirstName.Text = contact.FirstName;
            tbLastName.Text = contact.LastName;
            tbPhone.Text = contact.Phone;
            tbEmail.Text = contact.Email;
        }

        // Обработчики событий

        private void TbSearch_TextChanged(object sender, EventArgs e)
        {
            var query = tbSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(query))
            {
                _bindingSource.DataSource = _dataService.Contacts;
            }
            else
            {
                var filtered = _dataService.Contacts
                    .Where(c => c.FirstName.ToLower().Contains(query) ||
                                c.LastName.ToLower().Contains(query) ||
                                c.Phone.ToLower().Contains(query) ||
                                c.Email.ToLower().Contains(query))
                    .ToList();
                _bindingSource.DataSource = filtered;
            }
            dgvContacts.Refresh();
        }

        private void DgvContacts_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvContacts.SelectedRows.Count > 0 && dgvContacts.SelectedRows[0].DataBoundItem is Contact selected)
            {
                LoadContactToForm(selected);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            var contact = GetContactFromForm();
            _dataService.Add(contact);
            LoadData();
            ClearFields();
            MessageBox.Show("Контакт добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvContacts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите контакт для редактирования.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateFields()) return;

            var selected = dgvContacts.SelectedRows[0].DataBoundItem as Contact;
            if (selected != null)
            {
                var contact = GetContactFromForm();
                contact.Id = selected.Id;
                _dataService.Update(contact);
                LoadData();
                ClearFields();
                MessageBox.Show("Контакт обновлён!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvContacts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите контакт для удаления.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selected = dgvContacts.SelectedRows[0].DataBoundItem as Contact;
            if (selected != null)
            {
                if (MessageBox.Show($"Удалить контакт \"{selected.FullName}\"?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _dataService.Delete(selected.Id);
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dataService.Save();
        }
    }
}