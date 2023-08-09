


using System.Net.NetworkInformation;

namespace Downtime_Tracker {

    public partial class Main : Form {

        public List<IP> IPs = new List<IP>();
        private int cycles = 0;

        public Main() {
            InitializeComponent();

            IPs.Add(new IP(true, "A832S125", "DEV"));
            IPs.Add(new IP(true, "A832S126", "QA"));
            IPs.Add(new IP(true, "M5083208", "Meu PC"));

        }

        private void Main_Load(object sender, EventArgs e) {

            UpdateDataGrid();

        }

        private void UpdateDataGrid() {

            dataGridView1.Rows.Clear();

            foreach (IP p in IPs) {
                dataGridView1.Rows.Add(p.enabled, p.ip, p.name, p.timeout.ToString(), p.message);
            }

        }

        /* OCX - Tick */
        private async void timer1_Tick(object sender, EventArgs e) {
            //var tasks = db.Groups.ToList().Select(i => GetAdminsFromGroupAsync(i.Gid));
            //var results = await Task.WhenAll(tasks);

            var tasks = IPs.Select(i => i.PingIPAsync());

            await Task.WhenAll(tasks);

            UpdateDataGrid();

            textBox1.Text = cycles.ToString();

            cycles++;

        }

        /* Button - Start Process */
        private void button1_Click(object sender, EventArgs e) {

            if (button1.Text == "&Start Process") {

                button1.Text = "&Stop Process";

                timer1.Enabled = true;

            } else {

                button1.Text = "&Start Process";

                timer1.Enabled = false;

                cycles = 0;

                textBox1.Text = cycles.ToString();

            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) {

            if (dataGridView1.Rows.Count > 0 && e.RowIndex > -1) {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];

                IPs.Where(i => i.ip == row.Cells[1].Value.ToString()).First(s => s.enabled = Convert.ToBoolean(row.Cells[0].Value));
            }

        }
    }

}

public class IP {

    public bool enabled { get; set; }

    public string ip { get; set; }

    public string name { get; set; }

    public int timeout { get; set; }

    public string message { get; set; }

    public IP() {

        enabled = false;
        ip = "";
        name = "";
        timeout = 0;
        message = "";

    }

    public IP(bool enabled, string ip, string name) {

        this.enabled = enabled;
        this.ip = ip;
        this.name = name;
        timeout = 0;
        message = "";

    }

    public async Task PingIPAsync() {

        try {

            if (!enabled) {
                this.message = "Equipment Disabled";
                return;
            }

            var ping = new Ping();
            var reply = await ping.SendPingAsync(ip, 1000);

            if (reply != null) {
                this.message = $"Equipment is on air - Response: {reply.RoundtripTime} ms";
                this.timeout = 0;
            } else {
                this.message = $"Equipment Offline";
                this.timeout++;
            }

        } catch {
            this.message = $"Equipment Offline";
            this.timeout++;
        }
    }

}