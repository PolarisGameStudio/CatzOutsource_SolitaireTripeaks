namespace com.F4A.MobileThird
{
#if DEFINE_FIREBASE_AUTH
    using Firebase.Auth;
    using Firebase.Database;
    using Firebase.Extensions;
    using Firebase.Unity.Editor;
    using Newtonsoft.Json;
    using System;
#endif
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum EAccoutLogin
    {
        None,
        Email,
        Facebook,
    }

    public enum ETypeOrder
    {
        None,
        Child,
        Key,
        Value,
    }

    public partial class FirebaseManager
    {
#if DEFINE_FIREBASE_AUTH
        protected FirebaseAuth _auth;
        protected FirebaseUser _user;
        protected DatabaseReference _referenceDatabase;
#endif
        protected void InitializeAuth()
        {
#if DEFINE_FIREBASE_AUTH
            _auth = FirebaseAuth.DefaultInstance;
            //auth.StateChanged += Auth_StateChanged;
            //auth.IdTokenChanged += Auth_IdTokenChanged;

            if(!string.IsNullOrEmpty(_firebaseInfo.Urldatabase)) _firebaseApp.SetEditorDatabaseUrl(_firebaseInfo.Urldatabase);
            if (_firebaseApp.Options.DatabaseUrl != null) _firebaseApp.SetEditorDatabaseUrl(_firebaseApp.Options.DatabaseUrl);
            _referenceDatabase = FirebaseDatabase.DefaultInstance.RootReference;
#endif
        }

        protected void OnEnableAuth()
        {
            SocialManager.OnInitFacebookCompleted += SocialManager_OnInitFacebookCompleted;
        }

        public void OnDisableAuth()
        {
            SocialManager.OnInitFacebookCompleted -= SocialManager_OnInitFacebookCompleted;
        }

        private void SocialManager_OnInitFacebookCompleted()
        {
            if (SocialManager.Instance.IsLoginFacebook())
            {
                //SignInWithFacebook(SocialManager.Instance.GetFacebookAccessTokenString(), false);
            }
        }

        public bool IsSignIn()
        {
#if DEFINE_FIREBASE_AUTH
            return _auth != null && _auth.CurrentUser != null && _user != null && !string.IsNullOrEmpty(_user.UserId);
#else
            return false;
#endif
        }

        public void SignInWithFacebook(string accessToken, bool callDelegate = true)
        {
#if DEFINE_FIREBASE_AUTH
            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    Debug.Log("FirebaseManager.SignInWithFacebook");
#if DEFINE_FIREBASE_AUTH
                    Credential credential = FacebookAuthProvider.GetCredential(accessToken);
                    //_auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                    _auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled)
                        {
                            Debug.Log("@LOG F4A FirebaseManager.SignInWithFacebook/SignInWithCredentialAsync was canceled.");
                            if (callDelegate) OnLoginFacebookCompleted?.Invoke(false, "SignInWithFacebook was canceled");
                            return;
                        }
                        else if (task.IsFaulted)
                        {
                            Debug.Log("@LOG F4A FirebaseManager.SignInWithFacebook/SignInWithCredentialAsync encountered an error: " + task.Exception);
                            if (callDelegate) OnLoginFacebookCompleted?.Invoke(false, "SignInWithFacebook " + task.Exception.ToString());
                            return;
                        }
                        else
                        {
                            _user = task.Result;
                            Debug.LogFormat("@LOG F4A FirebaseManager.SignInWithFacebook User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
                            if (callDelegate) OnLoginFacebookCompleted?.Invoke(true, string.Empty);
                        }
                    });
#endif
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("FirebaseManager.SignInWithFacebook/SignInWithCredentialAsync encountered an error: " + ex);
                if (callDelegate) OnLoginFacebookCompleted?.Invoke(false, ex.Message);
            }
#endif
        }

        public void LogoutWithFacebook()
        {
#if DEFINE_FIREBASE_AUTH
            if(_auth != null && _auth.CurrentUser != null) _auth.SignOut();
            _user = null;
#endif
        }

        public void GetUserData(string databaseName, System.Action<bool, object> callBack)
        {
#if DEFINE_FIREBASE_AUTH
            if (IsSignIn())
            {
                GetUserData(databaseName, _user.UserId, callBack);
            }
            else
            {
                callBack?.Invoke(false, null);
            }
#endif
        }


        public void GetUserData(string databaseName, string uid, System.Action<bool, object> callBack)
        {
#if DEFINE_FIREBASE_AUTH
            DatabaseReference data = _referenceDatabase.Child(databaseName).Child(uid);
            if (data != null)
            {
                //data.GetValueAsync().ContinueWith(task =>
                // https://forum.unity.com/threads/can-only-be-called-from-the-main-thread.622948/
                data.GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    try
                    {
                        if (task == null || task.IsFaulted || task.IsCanceled)
                        {
                            callBack?.Invoke(false, null);
                        }
                        else if (task.IsCompleted)
                        {
                            // Do something with snapshot...
                            DataSnapshot snapshot = task.Result;
                            object value = null;
                            if (snapshot != null) value = snapshot.Value;
                            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                            {
                                Debug.Log("FirebaseManager.GetUserData value: " + value);
                                callBack?.Invoke(true, value);
                            }
                            else callBack?.Invoke(false, null);
                        }
                        else
                        {
                            callBack?.Invoke(false, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("FirebaseManager.GetUserData ex:" + ex);
                        callBack?.Invoke(false, null);
                    }
                });
            }
            else
            {
                callBack?.Invoke(false, null);
            }
#endif
        }

        public void GetData(string databaseName, string orderByChild = "", int limitFirst = 0, int limitLast = 0,
            string uid = "", System.Action<bool, object> callBack = null)
        {
#if DEFINE_FIREBASE_AUTH
            if (_referenceDatabase == null)
            {
                Debug.Log("@LOG FirebaseManager GetData _referenceDatabase null");
                return;
            }

            DatabaseReference data = _referenceDatabase.Child(databaseName);
            Query query = data;

            if (query != null)
            {
                if (limitFirst > 0) query = query.LimitToFirst(limitFirst);
                else if (limitLast > 0) query = query.LimitToLast(limitLast);
                if (!string.IsNullOrEmpty(orderByChild)) query = query.OrderByChild(orderByChild);
            }

            if (query != null)
            {
                //query.GetValueAsync().ContinueWith(task =>
                // https://forum.unity.com/threads/can-only-be-called-from-the-main-thread.622948/
                query.GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    try
                    {
                        if (task == null || task.IsFaulted || task.IsCanceled)
                        {
                            callBack?.Invoke(false, null);
                        }
                        else if (task.IsCompleted)
                        {
                            // Do something with snapshot...
                            DataSnapshot snapshot = task.Result;
                            object value = null;
                            if (snapshot != null) value = snapshot.Value;
                            if (value != null && !string.IsNullOrEmpty(value.ToString()))
                            {
                                Debug.Log("FirebaseManager.GetUserData value: " + value);
                                callBack?.Invoke(true, value);
                            }
                            else callBack?.Invoke(false, null);
                        }
                        else
                        {
                            callBack?.Invoke(false, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("FirebaseManager.GetUserData ex:" + ex);
                        callBack?.Invoke(false, null);
                    }
                });
            }
            else
            {
                callBack?.Invoke(false, null);
            }
#endif
        }

        public void SaveDatabase(string databaseName, string json, System.Action callBack = null, bool isAuth = true)
        {
#if DEFINE_FIREBASE_AUTH
            if (_referenceDatabase == null) return;

            if (isAuth)
            {
                if (!IsSignIn()) return;

                Debug.LogFormat("FirebaseManager.SaveDatabase User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
                var uid = _user.UserId;
                //if (_referenceDatabase != null) Debug.Log("FirebaseManager.SaveDatabase _referenceDatabase.Key:" + _referenceDatabase.Key);
                //var _userDatabase = _referenceDatabase.Child("user");
                //if (_userDatabase != null) Debug.Log("FirebaseManager.SaveDatabase _userDatabase.Key:" + _userDatabase.Key);
                Dictionary<string, object> childUpdates = new Dictionary<string, object>();
                childUpdates["/" + databaseName + "/" + uid] = json;
                _referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task =>
                //_referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWith(task =>
                {
                    callBack?.Invoke();
                });
            }
            else
            {
                Debug.Log($"FirebaseManager.SaveDatabase databaseName:{databaseName}");
                Dictionary<string, object> childUpdates = new Dictionary<string, object>();
                childUpdates["/" + databaseName] = json;
                _referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task =>
                //_referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWith(task =>
                {
                    callBack?.Invoke();
                });
            }
#endif
        }

        public void SaveDatabaseWithDictionary(string databaseName, Dictionary<string, object> childUpdates, 
            bool isAuth = true, System.Action callBack = null)
        {
#if DEFINE_FIREBASE_AUTH
            if (_referenceDatabase == null) return;
            Debug.Log($"FirebaseManager.SaveDatabaseWithDictionary databaseName:{databaseName}");
            try
            {
                if (isAuth)
                {
                    if (!IsSignIn()) return;

                    Debug.LogFormat("FirebaseManager.SaveDatabase User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
                    var uid = _user.UserId;
                    DatabaseReference data = _referenceDatabase.Child(databaseName + "/" + uid);
                    data.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task =>
                    //_referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWith(task =>
                    {
                        callBack?.Invoke();
                    });
                }
                else
                {
                    DatabaseReference data = _referenceDatabase;
                    if (!string.IsNullOrEmpty(databaseName)) data = _referenceDatabase.Child(databaseName);
                    if(data != null)
                    {
                        data.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task =>
                        //_referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWith(task =>
                        {
                            callBack?.Invoke();
                        });
                    }
                    else
                    {
                        Debug.Log($"FirebaseManager.SaveDatabaseWithDictionary can't get databaseName:{databaseName}".Color(Color.red));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"FirebaseManager.SaveDatabaseWithDictionary databaseName:{databaseName} error:{ex.Message}".Color(Color.red));
            }
#endif
        }

        public void SaveDatabaseWithObject(string databaseName, object value, bool isAuth, System.Action callBack = null)
        {
#if DEFINE_FIREBASE_AUTH
            if (_referenceDatabase == null) return;

            try
            {
                Debug.Log($"FirebaseManager.SaveDatabaseWithObject databaseName:{databaseName}");
                if (isAuth)
                {
                    if (!IsSignIn()) return;

                    Debug.LogFormat("FirebaseManager.SaveDatabaseWithObject User signed in successfully: {0} ({1})", _user.DisplayName, _user.UserId);
                    var uid = _user.UserId;
                    DatabaseReference data = _referenceDatabase.Child(databaseName + "/" + uid);
                    string str = JsonConvert.SerializeObject(value);
                    Dictionary<string, object> childUpdates = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);

                    data.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task =>
                    //_referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWith(task =>
                    {
                        callBack?.Invoke();
                    });
                }
                else
                {
                    DatabaseReference data = _referenceDatabase;
                    if (!string.IsNullOrEmpty(databaseName)) data = _referenceDatabase.Child(databaseName);

                    string str = JsonConvert.SerializeObject(value);
                    Dictionary<string, object> childUpdates = JsonConvert.DeserializeObject<Dictionary<string, object>>(str);

                    data.UpdateChildrenAsync(childUpdates).ContinueWithOnMainThread(task =>
                    //_referenceDatabase.UpdateChildrenAsync(childUpdates).ContinueWith(task =>
                    {
                        callBack?.Invoke();
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"FirebaseManager.SaveDatabaseWithObject databaseName:{databaseName} error:{ex.Message}".Color(Color.red));
            }
#endif
        }

        public void SubscribeValueChanged(string nameDatabase, string orderName = "", int limitFirst = 0)
        {
#if DEFINE_FIREBASE_AUTH
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(nameDatabase);
            Query query = null;
            if (!string.IsNullOrEmpty(orderName)) query = reference.OrderByValue();
            if (limitFirst > 0) query = query.LimitToFirst(limitFirst);

            if(query != null)
            {
                query.ValueChanged += FirebaseManager_ValueChanged;
            }
            else
            {
                reference.ValueChanged += FirebaseManager_ValueChanged;
            }
#endif
        }

        public void UnsubscribeValueChanged(string nameDatabase)
        {
#if DEFINE_FIREBASE_AUTH
            FirebaseDatabase.DefaultInstance.GetReference(nameDatabase).ValueChanged -= FirebaseManager_ValueChanged;
#endif
        }

#if DEFINE_FIREBASE_AUTH
        private void FirebaseManager_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Debug.Log("@LOG FirebaseManager_ValueChanged sender:" + sender);

            if (e.DatabaseError != null)
            {
                Debug.LogError("@LOG FirebaseManager_ValueChanged " + e.DatabaseError.Message);
                return;
            }
            // Do something with the data in args.Snapshot
            else
            {
                string str = JsonConvert.SerializeObject(e.Snapshot.Value);
                Debug.Log("@LOG FirebaseManager_ValueChanged str:" + str.Color(Color.blue));
            }
        }
#endif

        //private void Auth_IdTokenChanged(object sender, System.EventArgs e)
        //{
        //    FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        //    if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        //    {
        //        senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
        //          task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        //    }
        //}

        //private void Auth_StateChanged(object sender, System.EventArgs e)
        //{
        //    FirebaseAuth senderAuth = sender as FirebaseAuth;
        //    FirebaseUser user = null;
        //    if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        //    if (senderAuth == auth && senderAuth.CurrentUser != user)
        //    {
        //        bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
        //        if (!signedIn && user != null)
        //        {
        //            DebugLog("Signed out " + user.UserId);
        //        }
        //        user = senderAuth.CurrentUser;
        //        userByAuth[senderAuth.App.Name] = user;
        //        if (signedIn)
        //        {
        //            DebugLog("Signed in " + user.UserId);
        //            displayName = user.DisplayName ?? "";
        //            DisplayDetailedUserInfo(user, 1);
        //        }
        //    }
        //}
    }
}
