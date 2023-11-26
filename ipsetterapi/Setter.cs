namespace ipsetterapi
{
    public static class Setter
    {
        public static int SetIp(string ifname, string parameters)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            //startInfo.Arguments = "/C netsh interface ip set address name=eth3 static 192.168.5.10 255.255.255.0 192.168.5.1";
            startInfo.Arguments = $"/C netsh interface ip set address name={ifname} static {parameters}";
            startInfo.Verb = "runas";

            process.StartInfo = startInfo;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
