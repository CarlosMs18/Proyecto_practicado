namespace PracticeAngular.Models
{
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }  


        //calculamos el total
        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(x => x.BalanceDepositos);

        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);

        public decimal Total => BalanceDepositos - BalanceRetiros;



        public class TransaccionesPorFecha  //necesitamos tener las transacciones agruparas por fecha, para decirle por ejemplo :
                                    //el primero de noviembre hiciste tal cantidad de transacciones, el tercero hiciste 2 y asi
                                    // por eso crearemos una clase internaque me ayudaraa tener esa informaciona  mano
        {
            public DateTime FechaTransaccion { get; set;} //me habla a que fecha corresponen las transacciones ubicadas
             
            public IEnumerable<Transaccion> Transacciones { get; set;}

            

            
          //ahora necesitamos msotrar el balance de los depositos y retiros, cuanto deposito hoy y cuanto gasto hoy
          //con todo esto tenemos la informacion por fecha de  depositos y gastos
            public decimal BalanceDepositos => 
                            Transacciones.Where(x => x.TipoOperacionId == TipoOperacionEnum.Ingreso)
                            .Sum(x => x.Monto);

            public decimal BalanceRetiros =>
                            Transacciones.Where(x => x.TipoOperacionId == TipoOperacionEnum.Gasto)
                            .Sum(x => x.Monto);
        }

    }
}
