namespace UniPayCheck.CadastroApi.Context;

using Microsoft.EntityFrameworkCore;
using UniPayCheck.CadastroApi.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions) { }

    public DbSet<Pessoa> PESSOAS { get; set; }
    public DbSet<PessoaJuridica> PESSOAS_JURIDICAS { get; set; }
    public DbSet<PessoaFisica> PESSOAS_FISICAS { get; set; }

    protected override void OnConfiguring()
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>().HasKey(p => p.IdPessoa);
        modelBuilder.Entity<Pessoa>().Property(p => p.IdPessoa).HasColumnName("ID");
        modelBuilder.Entity<Pessoa>().Property(p => p.Bairro).HasColumnName("BAIRRO").HasColumnType("Varchar").HasMaxLength(40);
        modelBuilder.Entity<Pessoa>().Property(p => p.Cep).HasColumnName("CEP").HasColumnType("Char").HasMaxLength(8);
        modelBuilder.Entity<Pessoa>().Property(p => p.Cidade).HasColumnName("CIDADE").HasColumnType("Varchar").HasMaxLength(60).IsRequired();
        modelBuilder.Entity<Pessoa>().Property(p => p.Endereco).HasColumnName("ENDERECO").HasColumnType("Varchar").HasMaxLength(60).IsRequired();
        modelBuilder.Entity<Pessoa>().Property(p => p.Cep).HasColumnName("CEP").HasColumnType("Char").HasMaxLength(8);
        modelBuilder.Entity<Pessoa>().Property(p => p.Complemento).HasColumnName("COMPLEMENTO").HasColumnType("Varchar").HasMaxLength(30);
        modelBuilder.Entity<Pessoa>().Property(p => p.Numero).HasColumnName("NUMERO").HasColumnType("Varchar").HasMaxLength(10);
        modelBuilder.Entity<Pessoa>().Property(p => p.Uf).HasColumnName("UF").HasColumnType("Char").HasMaxLength(2).IsRequired();
        modelBuilder.Entity<Pessoa>().ToTable("PESSOAS");

        modelBuilder.Entity<PessoaFisica>().HasKey(pf => pf.IdPessoaFisica);
        modelBuilder.Entity<PessoaFisica>().Property(pf => pf.IdPessoaFisica).HasColumnName("ID");
        modelBuilder.Entity<PessoaFisica>().Property(pf => pf.IdPessoa).HasColumnName("IDPESSOA");
        modelBuilder.Entity<PessoaFisica>().Property(pf => pf.Cpf).HasColumnName("CPF").HasColumnType("Char").HasMaxLength(11).IsRequired();
        modelBuilder.Entity<PessoaFisica>().Property(pf => pf.Nome).HasColumnName("NOME").HasColumnType("Varchar").HasMaxLength(100).IsRequired();
        modelBuilder.Entity<PessoaFisica>().Property(pf => pf.Apelido).HasColumnName("APELIDO").HasColumnType("Varchar").HasMaxLength(100);
        modelBuilder.Entity<PessoaFisica>().Property(pf => pf.DataNascimento).HasColumnName("DATA_NASCIMENTO");
        modelBuilder.Entity<PessoaFisica>().ToTable("PESSOAS_FISICAS");

        modelBuilder.Entity<PessoaJuridica>().HasKey(pj => pj.IdPessoaJuridica);
        modelBuilder.Entity<PessoaJuridica>().Property(pj => pj.IdPessoaJuridica).HasColumnName("ID");
        modelBuilder.Entity<PessoaJuridica>().Property(pj => pj.IdPessoa).HasColumnName("IDPESSOA");
        modelBuilder.Entity<PessoaJuridica>().Property(pj => pj.Cnpj).HasColumnName("CNPJ").HasColumnType("Char").HasMaxLength(14).IsRequired();
        modelBuilder.Entity<PessoaJuridica>().Property(pj => pj.RazaoSocial).HasColumnName("RAZAO_SOCIAL").HasColumnType("Varchar").HasMaxLength(100).IsRequired();
        modelBuilder.Entity<PessoaJuridica>().Property(pj => pj.NomeFantasia).HasColumnName("NOME_FANTASIA").HasColumnType("Varchar").HasMaxLength(100);
        modelBuilder.Entity<PessoaJuridica>().Property(pj => pj.DataFundacao).HasColumnName("DATA_FUNDACAO");
        modelBuilder.Entity<PessoaJuridica>().ToTable("PESSOAS_JURIDICAS");
        
        modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);
        modelBuilder.Entity<Usuario>().Property(u => u.IdUsuario).HasColumnName("ID");
        modelBuilder.Entity<Usuario>().Property(u => u.Password).HasColumnName("PASSWORD").HasColumnType("NVarchar").HasMaxLength(512).IsRequired();
        modelBuilder.Entity<Usuario>().Property(u => u.Token).HasColumnName("TOKEN").HasColumnType("Varchar").HasMaxLength(75).IsRequired();
        modelBuilder.Entity<Usuario>().Property(u => u.UserName).HasColumnName("USERNAME").HasColumnType("Varchar").HasMaxLength(75).IsRequired();
        modelBuilder.Entity<Usuario>().ToTable("USUARIOS");

        //modelBuilder.Entity<Pessoa>().HasMany(
        //    pf => pf.PessoasFisicas).WithOne(
        //        p => p.Pessoa).OnDelete(
        //            DeleteBehavior.Cascade);

        //modelBuilder.Entity<Pessoa>().HasMany(
        //    pj => pj.PessoasJuridicas).WithOne(
        //        p => p.Pessoa).OnDelete(
        //            DeleteBehavior.Cascade);
    }
}

// https://stackoverflow.com/questions/43762053/how-to-dynamically-change-column-name-on-model-creating-in-entity-framework-cont
// https://stackoverflow.com/questions/21656617/entity-framework-code-first-changing-a-table-name