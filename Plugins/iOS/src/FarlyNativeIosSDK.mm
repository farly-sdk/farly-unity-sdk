#import "FarlyNativeIosSDK.h"
#import <Farly/Farly-Swift.h>

char const *GAME_OBJECT = "FarlyPluginBridge";

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
    if (string)
        return [NSString stringWithUTF8String: string];
    else
        return [NSString stringWithUTF8String: ""];
}

NSDictionary* toJSON(Action* action) {
    NSMutableDictionary* dict = [NSMutableDictionary new];
    dict[@"id"] = action.id;
    dict[@"amount"] = [NSNumber numberWithDouble: action.amount];
    dict[@"text"] = action.text;
    dict[@"html"] = action.html;
    return dict;
}

NSDictionary* toJSON(FeedElement* feedElement) {
    NSMutableDictionary* dict = [NSMutableDictionary new];
    dict[@"id"] = feedElement.id;
    dict[@"name"] = feedElement.name;
    dict[@"devName"] = feedElement.devName;
    dict[@"link"] = feedElement.link;
    dict[@"icon"] = feedElement.icon;
    dict[@"smallDescription"] = feedElement.smallDescription;
    dict[@"smallDescriptionHTML"] = feedElement.smallDescriptionHTML;
    NSMutableArray* actions = [NSMutableArray new];
    for (Action* action in feedElement.actions) {
        [actions addObject: toJSON(action)];
    }
    dict[@"actions"] = actions;
    return dict;
}

OfferWallRequest* ParseRequest (const char* string)
{
    NSString* json = CreateNSString(string);
    NSDictionary* dict = [NSJSONSerialization JSONObjectWithData:[json dataUsingEncoding:NSUTF8StringEncoding] options:0 error:nil];

    OfferWallRequest* req = [[OfferWallRequest alloc] initWithUserId:dict[@"userId"]];
    
    req.zipCode = dict[@"zipCode"];
    req.countryCode = dict[@"countryCode"];
    req.userAge = dict[@"userAge"];
    NSString* gender = dict[@"userGender"];
    if ([gender isEqualToString:@"m"]) {
        req.userGender = GenderMale;
    } else if ([gender isEqualToString:@"f"]) {
        req.userGender = GenderMale;
    }
    double userSignupTimestamp = [dict[@"userSignupDateTimestamp"] doubleValue];
    if (userSignupTimestamp != 0) {
        req.userSignupDate = [NSDate dateWithTimeIntervalSince1970:userSignupTimestamp];
    }
    req.callbackParameters = dict[@"callbackParameters"];
    
    return req;
}

extern "C" {
    void _configureFarly (const char* publisherId)
    {
        Farly.shared.publisherId = CreateNSString(publisherId);
    }

    void _getHostedOfferwallUrlFarly(const char* requestJson)
    {
        OfferWallRequest* req = ParseRequest(requestJson);
        NSURL *url = [Farly.shared getHostedOfferwallUrlWithRequest:req];
        NSString *result = url.absoluteString;
        UnitySendMessage(GAME_OBJECT, [@"HandleAsyncOfferwallUrlAnswer" UTF8String], [result UTF8String]);
    }

    void _showOfferwallInWebviewFarly(const char* requestJson)
    {
        OfferWallRequest* req = ParseRequest(requestJson);
        [Farly.shared showOfferwallInWebviewWithRequest:req presentingViewController:nil completion:nil];
    }

    void _showOfferwallInBrowserFarly(const char* requestJson)
    {
        OfferWallRequest* req = ParseRequest(requestJson);
        [Farly.shared showOfferwallInBrowserWithRequest:req completion:nil];
    }

    void _getOfferwallFarly(const char* requestJson)
    {
        OfferWallRequest* req = ParseRequest(requestJson);
        [Farly.shared getOfferWallWithRequest:req completion:^(NSString * error, NSArray<FeedElement *> *feedElements) {
            if (error != nil) {
                UnitySendMessage(GAME_OBJECT, [@"HandleException" UTF8String], [error UTF8String]);
                return;
            }
            NSMutableArray* array = [NSMutableArray new];
            for (FeedElement* feedElement in feedElements) {
                [array addObject:toJSON(feedElement)];
            }
            NSString *resultJSON = [[NSString alloc] initWithData:[NSJSONSerialization dataWithJSONObject:array options:0 error:nil] encoding:NSUTF8StringEncoding];
            UnitySendMessage(GAME_OBJECT, [@"HandleAsyncOfferwallAnswer" UTF8String], [resultJSON UTF8String]);
        }];
    }
}
