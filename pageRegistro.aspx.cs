﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;

public partial class pageRegistro : System.Web.UI.Page
{
    static String codigoActivacion;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void TextBox1_TextChanged(object sender, EventArgs e)
    {

    }

    protected void DropTipoUsuario_SelectedIndexChanged(object sender, EventArgs e)
    {
        switch (DropTipoUsuario.SelectedIndex)
        {
            case 0:
                break;
            case 1:
                break;
        }
    }
    public bool ValidacionCaptcha()
    {
        string ResponseByGoogle = Request["g-recaptcha-response"];

        string CaptchaSecretKey = "6LdjAncUAAAAACF2WCUuseOq6UnEMRw9dT1j-9JF";
        bool valid = false;

        // solicitud para el servidor de Google
        HttpWebRequest sol = (HttpWebRequest)WebRequest.Create("https://www.google.com/recaptcha/api/siteverify?secret=" +
            CaptchaSecretKey + "&response=" + ResponseByGoogle);

        try
        {
            // respuesta de Captcha
            using (WebResponse wResponse = sol.GetResponse())
            {
                using (StreamReader leeStream = new StreamReader(wResponse.GetResponseStream()))
                {
                    string jsonResponse = leeStream.ReadToEnd();

                    JavaScriptSerializer js = new JavaScriptSerializer();
                    validacionCaptcha data = js.Deserialize<validacionCaptcha>(jsonResponse);

                    valid = Convert.ToBoolean(data.sucess);
                }
            }
            return valid;
        }
        catch(WebException ex)
        {
            throw ex;
        }
     
    }


    protected void BtnCrearCuenta_Click(object sender, EventArgs e)
    {
        Random random = new Random();
        codigoActivacion = random.Next(1001, 9999).ToString();
        String consulta = "insert into Usuario(nombre, apellidos, contraseña, usuario, telefono, correo, descripcion," +
            "tipoUsuario, estadoConfEmail, codigoActivacion) values (@nombre,@apellidos,@contraseña,@usuario,@telefono,@correo,@descripcion,@tipoUsuario,@estadoConfEmail,@codigoActivacion)";
        String miCon = "Data Source=LAPTOP-OGDFKH4G; Initial Catalog=tour2visitors; Integrated Security=True";
        SqlConnection con = new SqlConnection(miCon);
        con.Open();
        SqlCommand cmd = new SqlCommand(consulta,con);
        cmd.CommandText = consulta;
        cmd.Connection = con;
        cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
        cmd.Parameters.AddWithValue("@apellidos",TxtApellidos.Text);
        cmd.Parameters.AddWithValue("@contraseña",TxtContraseña.Text);
        cmd.Parameters.AddWithValue("@usuario", TxtUsuario.Text);
        cmd.Parameters.AddWithValue("@telefono",TxtTelefono.Text);
        cmd.Parameters.AddWithValue("@correo",TxtCorreo.Text);
        cmd.Parameters.AddWithValue("@descripcion",TxtDescripcion.Text);
        cmd.Parameters.AddWithValue("@tipoUsuario",DropTipoUsuario.Text);
        cmd.Parameters.AddWithValue("@estadoConfEmail", "NoVerif");
        cmd.Parameters.AddWithValue("@codigoActivacion", codigoActivacion);
        cmd.ExecuteNonQuery();
        enviarCodigo();
        Response.Redirect("ActivacionEmail.aspx?emailadd=" + TxtCorreo.Text);

    }
    private void enviarCodigo()
    {
        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp.gmail.com";
        smtp.Port = 587;
        smtp.Credentials = new System.Net.NetworkCredential("tour2visitors@gmail.com", "ingenieriajira");
        smtp.EnableSsl = true;
        MailMessage msj = new MailMessage();
        msj.Subject = "Código de activación para verificar tu email";
        msj.Body = "Estimado/a " + TxtNombre.Text + " activa tu cuenta gracias al siguiente código de " +
            "activación " + codigoActivacion + " .Gracias y un saludo del equipo técnico de Tour2Visitors";
        string toadress = TxtCorreo.Text;
        msj.To.Add(toadress);
        string fromadress = "Tour2Visitors<tour2visitors@gmail.com>";
        msj.From = new MailAddress(fromadress);
        try
        {
            smtp.Send(msj);
        }
        catch
        {
            throw ;
        }
        
    }

    protected void TxtDescripcion_TextChanged(object sender, EventArgs e)
    {

    }
}
