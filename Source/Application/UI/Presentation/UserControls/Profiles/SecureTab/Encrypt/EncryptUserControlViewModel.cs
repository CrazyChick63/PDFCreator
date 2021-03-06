﻿using pdfforge.Obsidian;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Help;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using System.ComponentModel;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Encrypt
{
    public class EncryptUserControlViewModel : ProfileUserControlViewModel<EncryptUserControlTranslation>
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IUserGuideHelper _userGuideHelper;

        public EncryptUserControlViewModel
            (
                IInteractionRequest interactionRequest,
                EditionHintOptionProvider editionHintOptionProvider,
                IUserGuideHelper userGuideHelper,
                ITranslationUpdater translationUpdater,
                ISelectedProfileProvider selectedProfile
            )
        : base(translationUpdater, selectedProfile)
        {
            _interactionRequest = interactionRequest;
            _userGuideHelper = userGuideHelper;

            if (editionHintOptionProvider != null)
            {
                OnlyForPlusAndBusiness = editionHintOptionProvider.ShowOnlyForPlusAndBusinessHint;
            }

            SecurityPasswordCommand = new DelegateCommand(SecurityPasswordExecute);
        }

        public DelegateCommand SecurityPasswordCommand { get; }
        public bool OnlyForPlusAndBusiness { get; }

        public bool LowEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc128Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Rc128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool MediumEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes128Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool HighEncryptionEnabled
        {
            get { return CurrentProfile?.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes256Bit; }
            set
            {
                if (value)
                    CurrentProfile.PdfSettings.Security.EncryptionLevel = EncryptionLevel.Aes256Bit;
                RaisePropertyChangedForEncryptionProperties();
            }
        }

        public bool ExtendedPermissonsEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.EncryptionLevel != EncryptionLevel.Rc40Bit;
            }
        }

        public bool RestrictLowQualityPrintingEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality && ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.RestrictPrintingToLowQuality = value; }
        }

        public bool AllowFillFormsEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowToFillForms || !ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.AllowToFillForms = value; }
        }

        public bool AllowScreenReadersEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowScreenReader || !ExtendedPermissonsEnabled;
            }
            set { CurrentProfile.PdfSettings.Security.AllowScreenReader = value; }
        }

        public bool AllowEditingAssemblyEnabled
        {
            get
            {
                if (CurrentProfile == null)
                    return false;
                return CurrentProfile.PdfSettings.Security.AllowToEditAssembly || !ExtendedPermissonsEnabled;
            }

            set { CurrentProfile.PdfSettings.Security.AllowToEditAssembly = value; }
        }

        private void SecurityPasswordExecute(object obj)
        {
            var askUserPassword = CurrentProfile.PdfSettings.Security.RequireUserPassword;

            var interaction = new EncryptionPasswordInteraction(false, true, askUserPassword);
            interaction.OwnerPassword = CurrentProfile.PdfSettings.Security.OwnerPassword;
            interaction.UserPassword = CurrentProfile.PdfSettings.Security.UserPassword;

            _interactionRequest.Raise(interaction, securityPasswordsCallback);
        }

        private void securityPasswordsCallback(EncryptionPasswordInteraction interaction)
        {
            switch (interaction.Response)
            {
                case PasswordResult.StorePassword:
                    CurrentProfile.PdfSettings.Security.OwnerPassword = interaction.OwnerPassword;
                    CurrentProfile.PdfSettings.Security.UserPassword = interaction.UserPassword;
                    break;

                case PasswordResult.RemovePassword:
                    CurrentProfile.PdfSettings.Security.UserPassword = "";
                    CurrentProfile.PdfSettings.Security.OwnerPassword = "";
                    break;
            }
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);
            RaisePropertyChangedForEncryptionProperties();
        }

        public ICommand OpenPdfToolsUserGuideCommand => new DelegateCommand(OpenPdfToolsUserGuide);

        private void OpenPdfToolsUserGuide(object o)
        {
            _userGuideHelper.ShowHelp(HelpTopic.PdfTools);
        }

        private void RaisePropertyChangedForEncryptionProperties()
        {
            RaisePropertyChanged(nameof(LowEncryptionEnabled));
            RaisePropertyChanged(nameof(MediumEncryptionEnabled));
            RaisePropertyChanged(nameof(HighEncryptionEnabled));
            RaisePropertyChanged(nameof(ExtendedPermissonsEnabled));
            RaisePropertyChanged(nameof(RestrictLowQualityPrintingEnabled));
            RaisePropertyChanged(nameof(AllowFillFormsEnabled));
            RaisePropertyChanged(nameof(AllowScreenReadersEnabled));
            RaisePropertyChanged(nameof(AllowEditingAssemblyEnabled));
        }
    }
}
