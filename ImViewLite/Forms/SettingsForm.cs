using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ImViewLite.Settings;

namespace ImViewLite.Forms
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();

            foreach(UserControlledSettings s in InternalSettings.SettingProfiles)
            {
                cbProfiles.Items.Add(s);
            }
            cbProfiles.SelectedItem = InternalSettings.CurrentUserSettings;

            pgMain.SelectedObject = InternalSettings.CurrentUserSettings;
            hcMain.LoadBindings(InternalSettings.CurrentUserSettings._Binds);

            FormClosing += SettingsForm_FormClosing;
            cbProfiles.SelectedIndexChanged += new System.EventHandler(this.cbProfiles_SelectedIndexChanged);
        }

        #region Profiles / Internals

        private void cbProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            pgMain.SelectedObject = (UserControlledSettings)cbProfiles.SelectedItem;

            hcMain.GetBindings(InternalSettings.CurrentUserSettings._Binds);
            InternalSettings.CurrentUserSettings.UpdateBinds();
            InternalSettings.CurrentUserSettings = (UserControlledSettings)cbProfiles.SelectedItem;

            hcMain.LoadBindings(InternalSettings.CurrentUserSettings._Binds);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            UserControlledSettings newProfile = new UserControlledSettings();
            newProfile._Binds = InternalSettings.Default_Key_Binds.ToList();
            newProfile.ProfileName = "new profile " + (InternalSettings.SettingProfiles.Count + 1).ToString();
            newProfile.UpdateBinds();
            InternalSettings.SettingProfiles.Add(newProfile);
            cbProfiles.Items.Add(newProfile);

            cbProfiles.SelectedItem = newProfile;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            if (cbProfiles.Items.Count < 2)
                return;

            int indexToRemove = cbProfiles.SelectedIndex;

            InternalSettings.SettingProfiles.RemoveAt(indexToRemove);
            cbProfiles.Items.RemoveAt(indexToRemove);

            cbProfiles.SelectedIndex = 0;
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            hcMain.GetBindings(InternalSettings.CurrentUserSettings._Binds);
            InternalSettings.CurrentUserSettings.UpdateBinds();
            SettingsLoader.Save();
        }


        #endregion

        #region KeyBinds

        private void AddHotkey_Click(object sender, EventArgs e)
        {
            hcMain.AddRebind();
        }

        private void RemoveHotkey_Click(object sender, EventArgs e)
        {
            hcMain.RemoveRebind();
        }

        private void RestoreDefaultHotkeys_Click(object sender, EventArgs e)
        {
            hcMain.LoadBindings(InternalSettings.Default_Key_Binds.ToList());
        }

        #endregion
    }
}
