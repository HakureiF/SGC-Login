using Seer.DTO;
using Seer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using TouchSocket.Core;
using static Seer.DTO.Store;

namespace Seer
{
    public partial class BagStoreForm : Form
    {
        private readonly SendWebViewMess _sendWebViewMess;

        private int mimiId { get; set; } = 0;
        private List<Pet> petsShow { get; set; } = new();
        private List<BagStore> bagsStore { get; set; } = new();
        public BagStoreForm(SendWebViewMess sendWebViewMess)
        {
            InitializeComponent();
            _sendWebViewMess = sendWebViewMess;

            var mess1 = JsMessUtil<int>.MessJson("getSelfMimiId", 0);
            _sendWebViewMess(mess1);
            var mess = JsMessUtil<object>.MessJson("getBag", 0);
            _sendWebViewMess(mess);
        }

        private void initBagsStore()
        {
            bagStoreList.DataSource = null;
            if (mimiId == 0) return;
            var store = StoreUtil.getStore();
            //if (store == null || store.accounts == null || !store.accounts.ContainsKey(mimiId) || store.accounts[mimiId].bagsStore == null) return;
            bagsStore = store.accounts[mimiId].bagsStore;
            Debug.WriteLine(bagsStore);
            bagStoreList.DisplayMember = "label";
            bagStoreList.DataSource = bagsStore;
        }

        public void setMimiId(int id)
        {
            mimiId = id;
            initBagsStore();
        }

        public void freshStoreBag(Dictionary<string, JsonElement>? mess)
        {
            var pets = mess["data2"].Deserialize<List<Pet>>();
            if (pets != null && pets.Count > 0)
            {
                petsShow = pets;
                drawPetsHeads();
            }
        }

        private void drawPetsHeads()
        {
            foreach (Control c in Controls)
            {
                if (c.Name.StartsWith("petHead"))
                {
                    var box = (PictureBox)c;
                    box.ImageLocation = "";
                }
            }
            for (int i = 0; i < petsShow.Count; i++)
            {
                var pet = petsShow[i];
                foreach (Control c in Controls)
                {
                    if (c.Name.Equals("petHead" + i))
                    {
                        var box = (PictureBox)c;
                        box.ImageLocation = PetUtil.handleHeadUrl(pet.id);
                    }
                }
            }
        }

        private void storeButton_Click(object sender, EventArgs e)
        {
            if (mimiId == 0) return;
            var store = StoreUtil.getStore();
            /*            if (store == null) store = new Store();
                        if (store.accounts == null) store.accounts = new();*/

            SeerAccount account = new();
            if (store.accounts.ContainsKey(mimiId))
            {
                account = store.accounts[mimiId];
            }
            if (account.bagsStore == null)
            {
                account.bagsStore = new();
            }
            account.bagsStore.Add(new BagStore(namingTextBox.Text, petsShow));
            store.accounts[mimiId] = account;
            StoreUtil.setStore(store);

            initBagsStore();
        }

        private void freshButton_Click(object sender, EventArgs e)
        {
            var mess1 = JsMessUtil<int>.MessJson("getSelfMimiId", 0);
            _sendWebViewMess(mess1);
            var mess2 = JsMessUtil<object>.MessJson("getBag", 0);
            _sendWebViewMess(mess2);
        }

        private void bagStoreList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedBag = bagStoreList.SelectedItem as BagStore;
            if (selectedBag == null || selectedBag.pets == null) return;
            petsShow = selectedBag.pets;
            Debug.WriteLine(selectedBag);
            drawPetsHeads();
        }

        private void loadBagButton_Click(object sender, EventArgs e)
        {
            var mess = JsMessUtil<List<Pet>>.MessJson("reStoreBag", petsShow);
            _sendWebViewMess(mess);
        }

        private void deleteBagButton_Click(object sender, EventArgs e)
        {
            var selectedBag = bagStoreList.SelectedItem as BagStore;
            if (mimiId == 0) return;
            var store = StoreUtil.getStore();
            if (selectedBag == null || store == null || store.accounts == null || !store.accounts.ContainsKey(mimiId))
            {
                return;
            }
            SeerAccount account = store.accounts[mimiId];
            if (account.bagsStore == null) return;
            for (int i = 0; i < account.bagsStore.Count; i++)
            {
                if (selectedBag.label == account.bagsStore[i].label)
                {
                    account.bagsStore.RemoveAt(i);
                }
            }
            store.accounts[mimiId] = account;
            StoreUtil.setStore(store);

            initBagsStore();
        }
    }
}
