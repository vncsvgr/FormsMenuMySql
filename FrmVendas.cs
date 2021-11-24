﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace _23_09_2021  
{

    public partial class frmVendas : Form
    {
        MySqlConnection conexao;
        MySqlCommand comando;
        String banco = "server=localhost;port=3306;uid=root;pwd=etecjau;database=ds";
        MySqlDataAdapter Adaptador;
        DataTable datTabela;

        bool bloqueado = false;
        double total;

        public frmVendas()
        {
            InitializeComponent();
        }

        private void frmVendas_Load(object sender, EventArgs e)
        {
            try
            {
                conexao = new MySqlConnection(banco);
                conexao.Open();
                comando = new MySqlCommand("select cl.*, ci.nome cidade, ci.uf from clientes cl inner join cidades ci on " +
                                           "(ci.id = cl.id_cidade) order by nome", conexao);
                Adaptador = new MySqlDataAdapter(comando);
                Adaptador.Fill(datTabela = new DataTable());
                cboCliente.DataSource = datTabela;
                cboCliente.DisplayMember = "nome";
                cboCliente.ValueMember = "id";
                conexao.Close();

                conexao = new MySqlConnection(banco);
                conexao.Open();
                comando = new MySqlCommand("select * from produtos order by descricao", conexao);
                Adaptador = new MySqlDataAdapter(comando);
                Adaptador.Fill(datTabela = new DataTable());
                cboProdutos.DataSource = datTabela;
                cboProdutos.DisplayMember = "descricao";
                cboProdutos.ValueMember = "id";
                conexao.Close();

                btnCancelar.PerformClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                conexao.Close();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            dgvProdutos.RowCount = 0;
            cboCliente.SelectedIndex = -1;
            cboProdutos.SelectedIndex = -1;
            txtEndereco.Clear();
            txtCidade.Clear();
            txtUF.Clear();
            txtEstoque.Clear();
            txtPreco.Clear();
            txtQuantidade.Clear();
            mtbCelular.Clear();
            mtbCPF.Clear();
            mtbDataNascimento.Clear();
            picCliente.ImageLocation = "";
            picProduto.ImageLocation = "";
            total = 0;
            lblTotal.Text = total.ToString("C");
            grbClientes.Enabled = true;
            grbProdutos.Enabled = false;
        }

        private void cboCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboCliente.SelectedIndex != -1)
            {
                DataRowView reg = (DataRowView)cboCliente.SelectedIndex;
                txtEndereco.Text = reg["endereco"].ToString();
                txtCidade.Text = reg["cidade"].ToString();
                txtUF.Text = reg["uf"].ToString();
                mtbCPF.Text = reg["cpf"].ToString();
                mtbCelular.Text = reg["celular"].ToString();
                mtbDataNascimento.Text = reg["data_nascimento"].ToString();
                picCliente.ImageLocation = reg["foto"].ToString();
                bloqueado = bool.Parse(reg["bloqueado"].ToString());
            }
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            if (cboCliente.SelectedIndex != -1)
            {
                if (bloqueado == true)
                {
                    MessageBox.Show("Cliente bloqueado para venda", "Vendas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnCancelar.PerformClick();
                    return;
                }
                grbClientes.Enabled = false;
                grbProdutos.Enabled = true;
            }
        }

        private void cboProdutos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboProdutos.SelectedIndex != -1)
            {
                DataRowView reg = (DataRowView)cboProdutos.SelectedIndex;
                txtEstoque.Text = reg["estoque"].ToString();
                txtPreco.Text = reg["valor_venda"].ToString();
                picProduto.ImageLocation = reg["imagem"].ToString();
                
            }
        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(txtQuantidade.Text) > Convert.ToDouble(txtEstoque.Text))
            {
                MessageBox.Show("Estoque insuficiente", "Vendas", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantidade.SelectAll();
                return;
            }

            dgvProdutos.Rows.Add(cboProdutos.SelectedValue, cboProdutos.Text, txtQuantidade.Text, txtPreco.Text);

            double quantidade = double.Parse(txtQuantidade.Text);
            double preco = double.Parse(txtPreco.Text);

            total += quantidade * preco;
            lblTotal.Text = total.ToString("C");
            cboProdutos.SelectedIndex = -1;
            txtEstoque.Clear();
            txtPreco.Clear();
            txtQuantidade.Clear();
            picProduto.ImageLocation = "";

        }
    }
}
